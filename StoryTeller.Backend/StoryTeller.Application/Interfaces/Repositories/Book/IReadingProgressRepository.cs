using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book
{
    public interface IReadingProgressRepository
    {
        Task<ReadingProgress?> GetAsync(string userId, string bookId);
        Task UpsertAsync(ReadingProgress progress);
    }
}
