namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class ReadingProgress
    {
        public string Id => $"{UserId}:{BookId}";
        public string UserId { get; set; } = default!;
        public string BookId { get; set; } = default!;
        public int SectionIndex { get; set; }
    }
}
