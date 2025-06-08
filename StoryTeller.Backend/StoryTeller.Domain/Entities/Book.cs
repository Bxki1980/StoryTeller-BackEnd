using System.Drawing;
using System.Text.Json.Serialization;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class Book
    {
        [JsonPropertyName("id")] 
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("bookId")]
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string CoverImageBlobPath { get; set; } = string.Empty;

        public List<Page> Pages { get; set; } = new();

        //Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //optimistic concurrency control
        public string Version { get; set; } = "1.0.0";
    }
}
