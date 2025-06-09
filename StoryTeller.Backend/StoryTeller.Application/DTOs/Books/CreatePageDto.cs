using System.ComponentModel.DataAnnotations;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class CreatePageDto
    {
        [Required]
        public string SectionId { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public string ImageBlobPath { get; set; } = string.Empty;
        [Required]
        public string AudioBlobPath { get; set; } = string.Empty;
    }
}
