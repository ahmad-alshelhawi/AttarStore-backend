using AttarStore.Api.Utils;
using AttarStore.Entities;
using AttarStore.Entities.submodels;
using AttarStore.Models;
using AttarStore.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;


namespace AttarStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogInController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public LogInController(IUserRepository userRepository, TokenService tokenService, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginRequest)
        {
            var user = await _userRepository.GetByUserOrEmail(loginRequest.Name);

            // If user is not found or password doesn't match
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" }); // Sending error message with 401 status
            }

            // Generate tokens and update user
            var tokenResponse = await GenerateAndSaveTokens(user);

            // Fetch user roles
            var roles = await _userRepository.GetUserRoleByUsername(user.Name);

            // Include roles in the response
            var response = new
            {
                Token = tokenResponse,
                Roles = roles
            };

            return Ok(response); // 200 OK
        }




        private async Task<object> GenerateAndSaveTokens(IUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var accessTokenExpiryTime = DateTime.UtcNow.AddHours(1); // Assuming 1-hour expiration

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Assuming 7-day expiration

            // Save refresh token in the database (associate it with the user)
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _userRepository.UpdateUserAsync(user);

            // Return a serializable object with token and expiration details
            return new
            {
                token = accessToken,
                tokenExpiry = accessTokenExpiryTime,
                refreshToken = refreshToken,
                refreshTokenExpiry = refreshTokenExpiryTime
            };
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (string.IsNullOrWhiteSpace(tokenRequest.RefreshToken))
            {
                return BadRequest("Refresh token is missing or empty.");
            }

            // Fetch user by the provided refresh token
            var user = await _userRepository.GetByRefreshToken(tokenRequest.RefreshToken);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token.");
            }

            // Generate new access and refresh tokens
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var newAccessToken = _tokenService.GenerateAccessToken(claims);
            var newAccessTokenExpiryTime = DateTime.UtcNow.AddHours(1); // Assuming 1-hour expiration

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Assuming 7-day expiration

            // Update user's refresh token in the database
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newRefreshTokenExpiryTime;
            await _userRepository.UpdateUserAsync(user);

            var response = new
            {
                token = newAccessToken,
                tokenExpiry = newAccessTokenExpiryTime,
                refreshToken = newRefreshToken,
                refreshTokenExpiry = newRefreshTokenExpiryTime
            };

            return Ok(response);
        }


        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            // Check if the email exists in the system
            var user = await _userRepository.GetByUserEmail(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email address." }); // Return JSON with message.

            // Generate a password reset token
            var token = await _userRepository.GeneratePasswordResetTokenAsync(model.Email);
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Error generating reset token." }); // Return JSON with message.

            // Generate the reset link, including the token and email
            var resetLink = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";



            // Send the reset link via email
            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
            }
            catch (Exception ex)
            {
                // Log the error here and return a generic error message
                return StatusCode(500, new { message = "An error occurred while sending the email." }); // Return JSON with message.
            }

            return Ok(new { message = "Password reset link sent." }); // Return JSON with success message.
        }


        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            bool isReset = await _userRepository.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (!isReset)
                return BadRequest(new { message = "Invalid or expired token" }); // Return JSON with message.

            return Ok(new { message = "Password reset successful." }); // Return JSON with success message.
        }

    }
}
