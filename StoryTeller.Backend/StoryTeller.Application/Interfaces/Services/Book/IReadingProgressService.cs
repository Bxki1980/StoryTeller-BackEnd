using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book
{
    public interface IReadingProgressService
    {
        Task<ReadingProgressDto?> GetProgressAsync(string userId, string bookId);
        Task SaveProgressAsync(ReadingProgressDto dto);
    }
}
