using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace AttarStore.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public int AccessTokenValidityMinutes { get; }
        public int RefreshTokenValidityDays { get; }

        public TokenService(IConfiguration config)
        {
            _config = config;
            AccessTokenValidityMinutes = _config.GetValue<int>("JWT:AccessTokenMinutes");
            RefreshTokenValidityDays = _config.GetValue<int>("JWT:RefreshTokenDays");
        }

        public string GenerateAccessToken(string userId, string username, string role)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name,          username),
                new Claim(ClaimTypes.Role,          role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(AccessTokenValidityMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
