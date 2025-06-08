using AutoMapper;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IMapper _mapper;
        private readonly IBlobUrlGenerator _blobUrlGenerator;

        public PageService(IPageRepository pageRepository, IMapper mapper, IBlobUrlGenerator blobUrlGenerator)
        {
            _pageRepository = pageRepository;
            _mapper = mapper;
            _blobUrlGenerator = blobUrlGenerator;
        }

        public async Task<List<PageDto>> GetPagesByBookIdAsync(string bookId)
        {
            var pages = await _pageRepository.GetPagesByBookIdAsync(bookId);
            return pages.Select(page =>
            {
                var dto = _mapper.Map<PageDto>(page);
                dto.ImageUrl = _blobUrlGenerator.GenerateSasUrl(page.ImageBlobPath);
                dto.AudioUrl = _blobUrlGenerator.GenerateSasUrl(page.AudioBlobPath);
                return dto;
            }).ToList();
        }

        public async Task<PageDto?> GetBySectionIdAsync(string bookId, string sectionId)
        {
            var page = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (page == null) return null;

            var dto = _mapper.Map<PageDto>(page);
            dto.ImageUrl = _blobUrlGenerator.GenerateSasUrl(page.ImageBlobPath);
            dto.AudioUrl = _blobUrlGenerator.GenerateSasUrl(page.AudioBlobPath);
            return dto;
        }

        public async Task<PageDto> CreateAsync(string bookId, CreatePageDto dto)
        {
            var page = _mapper.Map<Page>(dto);
            page.BookId = bookId;
            page.CreatedAt = DateTime.UtcNow;
            page.Id = GeneratePageId(bookId, dto.SectionId);

            await _pageRepository.CreateAsync(page);
            var result = _mapper.Map<PageDto>(page);
            result.ImageUrl = _blobUrlGenerator.GenerateSasUrl(page.ImageBlobPath);
            result.AudioUrl = _blobUrlGenerator.GenerateSasUrl(page.AudioBlobPath);
            return result;
        }

        public async Task<PageDto?> UpdateAsync(string bookId, string sectionId, CreatePageDto dto)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = GeneratePageId(bookId, sectionId);
            existing.BookId = bookId;

            await _pageRepository.UpdateAsync(existing);
            var result = _mapper.Map<PageDto>(existing);
            result.ImageUrl = _blobUrlGenerator.GenerateSasUrl(existing.ImageBlobPath);
            result.AudioUrl = _blobUrlGenerator.GenerateSasUrl(existing.AudioBlobPath);
            return result;
        }

        public async Task<bool> DeleteAsync(string bookId, string sectionId)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null) return false;

            await _pageRepository.DeleteAsync(bookId, sectionId);
            return true;
        }

        private static string GeneratePageId(string bookId, string sectionId) =>
            $"{bookId}_{sectionId}";
    }
}
