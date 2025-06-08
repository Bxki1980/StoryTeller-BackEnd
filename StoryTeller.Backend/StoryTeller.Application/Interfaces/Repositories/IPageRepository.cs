using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories
{
    public interface IPageRepository
    {
        Task<List<Page>> GetPagesByBookIdAsync(string bookId);
        Task<Page?> GetBySectionIdAsync(string bookId, string sectionId);
        Task CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(string bookId, string sectionId);
    }
}
