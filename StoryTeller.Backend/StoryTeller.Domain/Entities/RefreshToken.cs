using Newtonsoft.Json;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class RefreshToken
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("expires")]
        public DateTime Expires { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonProperty("revoked")]
        public bool Revoked { get; set; } = false;
    }
}
