using AutoMapper;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IMapper _mapper;

        public PageService(IPageRepository pageRepository, IMapper mapper)
        {
            _pageRepository = pageRepository;
            _mapper = mapper;
        }

        public async Task<List<PageDto>> GetPagesByBookIdAsync(string bookId)
        {
            var pages = await _pageRepository.GetPagesByBookIdAsync(bookId);
            return _mapper.Map<List<PageDto>>(pages);
        }

        public async Task<PageDto?> GetBySectionIdAsync(string bookId, string sectionId)
        {
            var page = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            return page == null ? null : _mapper.Map<PageDto>(page);
        }

        public async Task<PageDto> CreateAsync(string bookId, CreatePageDto dto)
        {
            var page = _mapper.Map<Page>(dto);
            page.BookId = bookId;
            page.CreatedAt = DateTime.UtcNow;
            page.Id = GeneratePageId(bookId, dto.SectionId);

            await _pageRepository.CreateAsync(page);
            return _mapper.Map<PageDto>(page);
        }

        public async Task<PageDto?> UpdateAsync(string bookId, string sectionId, CreatePageDto dto)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);
            existing.Id = GeneratePageId(bookId, sectionId); // for upsert
            existing.BookId = bookId;

            await _pageRepository.UpdateAsync(existing);
            return _mapper.Map<PageDto>(existing);
        }

        public async Task<bool> DeleteAsync(string bookId, string sectionId)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null)
                return false;

            await _pageRepository.DeleteAsync(bookId, sectionId);
            return true;
        }

        private static string GeneratePageId(string bookId, string sectionId) =>
            $"{bookId}_{sectionId}";
    }
}
