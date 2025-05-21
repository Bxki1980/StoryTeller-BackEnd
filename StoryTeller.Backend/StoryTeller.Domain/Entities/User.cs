using Newtonsoft.Json;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    using StoryTeller.Domain.Enums;

    
    public class User
    {
        [JsonProperty("id")]
        public String Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("email")]
        public String Emial { get; set; } = String.Empty;
        [JsonProperty("passwordHash")]
        public String PasswordHash { get; set; } = String.Empty;
        [JsonProperty("role")]
        public UserRole Role { get; set; } = UserRole.Free;
    }
}
