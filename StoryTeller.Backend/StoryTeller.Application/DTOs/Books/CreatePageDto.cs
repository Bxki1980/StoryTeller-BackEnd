namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class CreatePageDto
    {
        public string SectionId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageBlobPath { get; set; } = string.Empty;
        public string AudioBlobPath { get; set; } = string.Empty;
    }
}
