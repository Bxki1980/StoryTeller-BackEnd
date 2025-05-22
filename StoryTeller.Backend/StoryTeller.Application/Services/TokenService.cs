using System.Security.Cryptography;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services
{
    public class TokenService
    {
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
    }
}
