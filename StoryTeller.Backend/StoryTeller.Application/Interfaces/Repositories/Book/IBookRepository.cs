using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book
{
    public interface IBookRepository
    {
        Task<StoryTeller.Domain.Entities.Book?> GetByBookIdAsync(string bookId);
        Task<List<StoryTeller.Domain.Entities.Book>> GetAllAsync();
        Task CreateAsync(StoryTeller.Domain.Entities.Book book);
        Task UpdateAsync(StoryTeller.Domain.Entities.Book book);
        Task DeleteAsync(string bookId);
    }
}
