﻿namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books
{
    public class BookCoverDto
    {
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
    }
}
