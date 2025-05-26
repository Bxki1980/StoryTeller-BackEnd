using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Security.Claims;
using System.Text;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using Microsoft.Extensions.Options;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth

{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = _jwtSettings.ExpiresInMinutes;
            if (expiryMinutes <= 0) {
                throw new Exception("Jwt:ExpiresInMinutes is missing in configuration");
            }
            var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
