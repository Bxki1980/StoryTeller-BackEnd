using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book
{
    public interface IPageService
    {
        Task<List<PageDto>> GetPagesByBookIdAsync(string bookId);
        Task<PaginatedContinuationResponse<PageDto>> GetPaginatedPagesByBookIdAsync(string bookId, PageQueryParameters queryParams);
        Task<PageDto?> GetBySectionIdAsync(string bookId, string sectionId);
        Task<PageDto> CreateAsync(string bookId, CreatePageDto dto);
        Task<PageDto?> UpdateAsync(string bookId, string sectionId, CreatePageDto dto);
        Task<bool> DeleteAsync(string bookId, string sectionId);
        Task<List<PageDto>> CreateBatchAsync(string bookId, List<CreatePageDto> dtos);
        Task<bool> DeleteAllAsync(string bookId);
    }
}
