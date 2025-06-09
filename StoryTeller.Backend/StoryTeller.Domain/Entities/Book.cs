using System.Drawing;
using Newtonsoft.Json;


namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class Book
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty(PropertyName = "bookId")]
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string CoverImageBlobPath { get; set; } = string.Empty;

        //Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //optimistic concurrency control
        public string Version { get; set; } = "1.0.0";
    }
}
