using System.Text.Json.Serialization;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class Page
    {
        [JsonPropertyName("id")] 
        public string Id { get; set; } = string.Empty;         
        public string BookId { get; set; } = string.Empty;

        public string SectionId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Relative paths preferred — handled by service for full URLs
        public string ImageBlobPath { get; set; } = string.Empty;
        public string AudioBlobPath { get; set; } = string.Empty;

        // tracking metadata filtering
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
