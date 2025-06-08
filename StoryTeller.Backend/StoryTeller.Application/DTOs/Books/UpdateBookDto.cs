namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class UpdateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string CoverImageBlobPath { get; set; } = string.Empty;

        public List<CreatePageDto>? Pages { get; set; } 
    }
}
