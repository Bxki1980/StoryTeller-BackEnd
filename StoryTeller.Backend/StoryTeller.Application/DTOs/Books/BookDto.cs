namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class BookDto
    {
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;

        public List<PageDto> Pages { get; set; } = new(); // Keep for frontend convenience

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0.0";
    }
}
