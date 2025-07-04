﻿using System.ComponentModel.DataAnnotations;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class CreateBookDto
    {
        public string BookId { get; set; } = string.Empty;
        [Required]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string AgeRange { get; set; } = string.Empty;
        [Required]
        public string CoverImageBlobPath { get; set; } = string.Empty;

    }
}
