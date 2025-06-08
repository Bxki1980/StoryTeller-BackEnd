namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class PageDto
    {
        public string SectionId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; }
    }
}
