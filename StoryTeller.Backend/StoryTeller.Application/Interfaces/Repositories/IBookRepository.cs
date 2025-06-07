using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> GetByBookIdAsync(string bookId);
        Task<List<Book>> GetAllAsync();
        Task CreateAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string bookId);
    }
}
