using AttarStore.Api.Utils;
using AttarStore.Entities;
using AttarStore.Entities.submodels;
using AttarStore.Models;
using AttarStore.Services;
using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace AttarStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogInController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly AppDbContext _dbcontext;
        private readonly TokenService _tokenService;
        private readonly IEmailSender _emailSender;

        // In-memory store for refresh tokens (replace with DB for production)
        private static readonly ConcurrentDictionary<string, string> RefreshTokens = new();

        public LogInController(IUserRepository userRepository, TokenService tokenService, IEmailSender emailSender, AppDbContext dbcontext, IAdminRepository adminRepository, IClientRepository clientRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _dbcontext = dbcontext;
            _adminRepository = adminRepository;
            _clientRepository = clientRepository;
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginRequest)
        {
            // Check Admin
            var admin = await _adminRepository.GetByUserOrEmail(loginRequest.Name);
            if (admin != null && BCrypt.Net.BCrypt.Verify(loginRequest.Password, admin.Password))
            {
                var tokenResponse = await GenerateAndSaveTokens(admin.Id.ToString(), admin, "Admin");
                var roles = await _adminRepository.GetAdminRoleByUsername(admin.Name);

                return Ok(new
                {
                    Token = tokenResponse,
                    Roles = roles,
                    Role = "Admin"
                });
            }

            // Check User
            var user = await _userRepository.GetByUserOrEmail(loginRequest.Name);
            if (user != null && BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                var tokenResponse = await GenerateAndSaveTokens(user.Id.ToString(), user, "User");
                var roles = await _userRepository.GetUserRoleByUsername(user.Name);

                return Ok(new
                {
                    Token = tokenResponse,
                    Roles = roles,
                    Role = "User"
                });
            }
            var client = await _clientRepository.GetByClientOrEmail(loginRequest.Name);
            if (client != null && BCrypt.Net.BCrypt.Verify(loginRequest.Password, client.Password))
            {
                var tokenResponse = await GenerateAndSaveTokens(client.Id.ToString(), client, "Client");
                var roles = await _clientRepository.GetClientRoleByUsername(client.Name);

                return Ok(new
                {
                    Token = tokenResponse,
                    Roles = roles,
                    Role = "Client"
                });
            }

            // If no valid login
            return Unauthorized(new { message = "Invalid username or password" });
        }

        private async Task<object> GenerateAndSaveTokens(string userId, IUser user, string role)
        {
            var accessToken = _tokenService.GenerateAccessToken(userId, user.Name, role);
            var accessTokenExpiryTime = DateTime.UtcNow.AddHours(1);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

            switch (role)
            {
                case "Admin":
                    await _adminRepository.UpdateAdminAsync((Admin)user);
                    break;
                case "User":
                    await _userRepository.UpdateUserAsync((User)user);
                    break;
                case "Client":
                    await _clientRepository.UpdateClientAsync((Client)user);
                    break;
            }

            return new
            {
                token = accessToken,
                tokenExpiry = accessTokenExpiryTime,
                refreshToken = refreshToken,
                refreshTokenExpiry = refreshTokenExpiryTime
            };
        }




        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenResponse model)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
                return BadRequest(new { message = "Invalid access token" });

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = principal.Identity?.Name;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || username == null || role == null)
                return BadRequest(new { message = "Token is missing required claims" });

            if (!int.TryParse(userIdClaim, out int userId)) // Convert userId from string to int
                return BadRequest(new { message = "Invalid user ID" });

            IUser user = null;

            if (role == "Admin")
            {
                // Fetch the admin by their user ID
                user = await _adminRepository.GetAdminById(userId);
            }
            else if (role == "User")
            {
                // Fetch the user by their user ID
                user = await _userRepository.GetUserById(userId);
            }
            else if (role == "Client")
            {
                // Fetch the client by their user ID
                user = await _clientRepository.GetClientById(userId);
            }

            // Check if the user exists and the refresh token is valid
            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest(new { message = "Invalid or expired refresh token" });

            var newAccessToken = _tokenService.GenerateAccessToken(userId.ToString(), username, role); // Pass userId as string for token generation
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpiry = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = refreshExpiry;

            // Update the appropriate repository based on the role
            if (role == "Admin")
                await _adminRepository.UpdateAdminAsync((Admin)user);
            else if (role == "User")
                await _userRepository.UpdateUserAsync((User)user);
            else if (role == "Client")
                await _clientRepository.UpdateClientAsync((Client)user);

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken,
                refreshTokenExpiry = refreshExpiry
            });
        }






        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            // Check for Admin
            var admin = await _adminRepository.GetByAdminEmail(model.Email);
            if (admin != null)
            {
                var token = await _adminRepository.GeneratePasswordResetTokenAsync(model.Email);
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

            // Check for Client
            var client = await _clientRepository.GetByClientEmail(model.Email);
            if (client != null)
            {
                var token = await _clientRepository.GeneratePasswordResetTokenAsync(model.Email);
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

            // Check for User
            var user = await _userRepository.GetByUserEmail(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email address." });

            var userToken = await _userRepository.GeneratePasswordResetTokenAsync(model.Email);
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
            // Check for Admin
            var admin = await _adminRepository.GetByAdminEmail(model.Email);
            if (admin != null)
            {
                bool isReset = await _adminRepository.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
                if (!isReset)
                    return BadRequest(new { message = "Invalid or expired token" });

                return Ok(new { message = "Password reset successful for Admin." });
            }

            // Check for Client
            var client = await _clientRepository.GetByClientEmail(model.Email);
            if (client != null)
            {
                bool isReset = await _clientRepository.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
                if (!isReset)
                    return BadRequest(new { message = "Invalid or expired token" });

                return Ok(new { message = "Password reset successful for Client." });
            }

            // Check for User
            var user = await _userRepository.GetByUserEmail(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email address." });

            bool isUserReset = await _userRepository.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (!isUserReset)
                return BadRequest(new { message = "Invalid or expired token" });

            return Ok(new { message = "Password reset successful for User." });
        }


    }
}
