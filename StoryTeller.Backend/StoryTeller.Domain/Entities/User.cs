using Newtonsoft.Json;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    using StoryTeller.Domain.Enums;

    
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;


        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
        
        
        [JsonProperty("role")]
        public UserRole Role { get; set; } = UserRole.Free;

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }


        [JsonProperty("externalProvider")]
        public string? ExternalProvider { get; set; }


        [JsonProperty("externalId")]
        public string? ExternalId { get; set; }



        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PictureUrl { get; set; }
        public string? Locale { get; set; }


    }
}
