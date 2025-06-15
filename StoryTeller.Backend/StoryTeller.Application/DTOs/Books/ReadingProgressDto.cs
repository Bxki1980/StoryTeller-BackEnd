namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class ReadingProgressDto
    {
        public string UserId { get; set; } = default!;
        public string BookId { get; set; } = default!;
        public int SectionIndex { get; set; }
    }
}
