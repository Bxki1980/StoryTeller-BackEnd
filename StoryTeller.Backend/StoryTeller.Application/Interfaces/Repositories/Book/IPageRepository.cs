using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book
{
    public interface IPageRepository
    {
        Task<List<Page>> GetPagesByBookIdRawAsync(string bookId);
        Task<Page?> GetBySectionIdAsync(string bookId, string sectionId);
        Task CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(string bookId, string sectionId);
        Task CreateManyAsync(List<Page> pages);
        Task<PaginatedContinuationResponse<PageDto>> GetPaginatedPagesByBookIdAsync(string bookId, PageQueryParameters queryParams);
    }
}
