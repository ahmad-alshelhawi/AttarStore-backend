using AttarStore.Api.Utils;
using AttarStore.Entities;
using AttarStore.Entities.submodels;
using AttarStore.Models;
using AttarStore.Repositories;
using AttarStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AttarStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly TokenService _tokenService;
        private readonly IEmailSender _emailSender;


        public AuthController(
            IUserRepository userRepo,
            IAdminRepository adminRepo,
            IClientRepository clientRepo,
            IRefreshTokenRepository rtRepo,
            TokenService tokenService,
            IEmailSender emailSender)
        {
            _userRepo = userRepo;
            _adminRepo = adminRepo;
            _clientRepo = clientRepo;
            _rtRepo = rtRepo;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel creds)
        {
            // 1) Try each store in turn, but *only* accept if password verifies
            IUser foundUser = null;
            string role = null;

            // --- Admin check ---
            var admin = await _adminRepo.GetByUserOrEmail(creds.Name);
            if (admin != null && BCrypt.Net.BCrypt.Verify(creds.Password, admin.Password))
            {
                foundUser = admin;
                role = "Master";
            }
            else
            {
                // --- Regular user check ---
                var user = await _userRepo.GetByUserOrEmail(creds.Name);
                if (user != null && BCrypt.Net.BCrypt.Verify(creds.Password, user.Password))
                {
                    foundUser = user;
                    role = user.Role;    // could be “Admin” or “User” depending on your design
                }
                else
                {
                    // --- Client check ---
                    var client = await _clientRepo.GetByClientOrEmail(creds.Name);
                    if (client != null && BCrypt.Net.BCrypt.Verify(creds.Password, client.Password))
                    {
                        foundUser = client;
                        role = "Client";
                    }
                }
            }

            // 2) If nothing matched, bail out *before* issuing tokens
            if (foundUser == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // 3) Generate and persist your tokens
            var accessToken = _tokenService.GenerateAccessToken(foundUser.Id.ToString(), foundUser.Name, role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _rtRepo.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                Role = role,
                UserId = (role == "User" || role == "Admin") ? (int?)foundUser.Id : null,
                AdminId = (role == "Master") ? (int?)foundUser.Id : null,
                ClientId = (role == "Client") ? (int?)foundUser.Id : null,
                ExpiryDate = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays),
                IsRevoked = false
            });

            // 4) Only now set cookies
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays)
            });

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(_tokenService.AccessTokenValidityMinutes)
            });

            // 5) Return only the metadata
            return Ok(new
            {
                expiresIn = _tokenService.AccessTokenValidityMinutes
            });
        }



        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var oldToken))
                return Unauthorized();

            var stored = await _rtRepo.GetByTokenAsync(oldToken);
            if (stored == null || stored.IsRevoked || stored.ExpiryDate <= DateTime.UtcNow)
                return Unauthorized();

            stored.IsRevoked = true;
            await _rtRepo.UpdateAsync(stored);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Role = stored.Role,
                UserId = stored.UserId,
                AdminId = stored.AdminId,
                ClientId = stored.ClientId,
                ExpiryDate = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays),
                IsRevoked = false
            };
            await _rtRepo.CreateAsync(newEntity);

            var newAccessToken = _tokenService.GenerateAccessToken(
                (stored.AdminId ?? stored.UserId ?? stored.ClientId).ToString(),
                "",
                stored.Role);

            // Set cookies
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = newEntity.ExpiryDate
            });

            Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(_tokenService.AccessTokenValidityMinutes)
            });

            return Ok(new
            {
                expiresIn = _tokenService.RefreshTokenValidityDays
            });
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
                return Unauthorized();

            object profile;
            switch (role)
            {
                case "Master":
                    var admin = await _adminRepo.GetAdminById(userId);
                    profile = new
                    {
                        id = admin.Id,
                        name = admin.Name,
                        role
                    };
                    break;

                case "Client":
                    var client = await _clientRepo.GetClientById(userId);
                    profile = new
                    {
                        id = client.Id,
                        name = client.Name,
                        role
                    };
                    break;

                default:
                    var user = await _userRepo.GetUserById(userId);
                    profile = new
                    {
                        id = user.Id,
                        name = user.Name,
                        role = user.Role
                    };
                    break;
            }

            return Ok(profile);
        }



        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // --- 1) Revoke refresh token in your database ---
            if (Request.Cookies.TryGetValue("refreshToken", out var rtCookie))
            {
                var rt = await _rtRepo.GetByTokenAsync(rtCookie);
                if (rt != null)
                {
                    rt.IsRevoked = true;
                    await _rtRepo.UpdateAsync(rt);
                }
            }

            // --- 2) Clear both cookies at the root path ---
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,  // if your frontend is on a different domain/origin
                Path = "/"
            };

            // Delete the refresh token cookie
            Response.Cookies.Delete("refreshToken", cookieOptions);

            // Delete the access token cookie
            Response.Cookies.Delete("accessToken", cookieOptions);

            return NoContent();
        }


        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            var admin = await _adminRepo.GetByAdminEmail(model.Email);
            if (admin != null)
            {
                var token = await _adminRepo.GeneratePasswordResetTokenAsync(model.Email);
                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Error generating reset token." });

                var resetLink = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";
                try
                {
                    await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
                }
                catch
                {
                    return StatusCode(500, new { message = "An error occurred while sending the email." });
                }

                return Ok(new { message = "Password reset link sent for Admin." });
            }

            var client = await _clientRepo.GetByClientEmail(model.Email);
            if (client != null)
            {
                var token = await _clientRepo.GeneratePasswordResetTokenAsync(model.Email);
                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Error generating reset token." });

                var resetLink = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";
                try
                {
                    await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
                }
                catch
                {
                    return StatusCode(500, new { message = "An error occurred while sending the email." });
                }

                return Ok(new { message = "Password reset link sent for Client." });
            }

            var user = await _userRepo.GetByUserEmail(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email address." });

            var userToken = await _userRepo.GeneratePasswordResetTokenAsync(model.Email);
            if (string.IsNullOrEmpty(userToken))
                return BadRequest(new { message = "Error generating reset token." });

            var userResetLink = $"{AppConstants.PasswordResetBaseUrl}?token={userToken}&email={model.Email}";
            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click here to reset your password: {userResetLink}");
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while sending the email." });
            }

            return Ok(new { message = "Password reset link sent for User." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var admin = await _adminRepo.GetByAdminEmail(model.Email);
            if (admin != null)
            {
                bool isReset = await _adminRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
                if (!isReset)
                    return BadRequest(new { message = "Invalid or expired token" });

                return Ok(new { message = "Password reset successful for Admin." });
            }

            var client = await _clientRepo.GetByClientEmail(model.Email);
            if (client != null)
            {
                bool isReset = await _clientRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
                if (!isReset)
                    return BadRequest(new { message = "Invalid or expired token" });

                return Ok(new { message = "Password reset successful for Client." });
            }

            var user = await _userRepo.GetByUserEmail(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email address." });

            bool isUserReset = await _userRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (!isUserReset)
                return BadRequest(new { message = "Invalid or expired token" });

            return Ok(new { message = "Password reset successful for User." });
        }


    }
}
