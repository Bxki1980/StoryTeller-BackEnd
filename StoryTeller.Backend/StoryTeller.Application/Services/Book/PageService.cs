using AutoMapper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PageService> _logger;

        public PageService(IPageRepository pageRepository, IMapper mapper, IBlobUrlGenerator blobUrlGenerator, ILogger<PageService> logger)
        {
            _pageRepository = pageRepository;
            _mapper = mapper;
            _blobUrlGenerator = blobUrlGenerator;
            _logger = logger;
        }

        public async Task<List<PageDto>> GetPagesByBookIdAsync(string bookId)
        {
            var pages = await _pageRepository.GetPagesByBookIdAsync(bookId);
            var dtos = pages.Select(page =>
            {
                var dto = _mapper.Map<PageDto>(page);
                dto.ImageUrl = _blobUrlGenerator.GenerateSasUrl(page.ImageBlobPath);
                dto.AudioUrl = _blobUrlGenerator.GenerateSasUrl(page.AudioBlobPath);
                return dto;
            }).ToList();

            _logger.LogInformation("Fetched {Count} pages for BookId={BookId}", dtos.Count, bookId);
            return dtos;
        }

        public async Task<PageDto?> GetBySectionIdAsync(string bookId, string sectionId)
        {
            var page = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (page == null)
            {
                _logger.LogWarning("Page not found: BookId={BookId}, SectionId={SectionId}", bookId, sectionId);
                return null;
            }

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
            _logger.LogInformation("Page created: BookId={BookId}, SectionId={SectionId}", bookId, dto.SectionId);

            var result = _mapper.Map<PageDto>(page);
            result.ImageUrl = _blobUrlGenerator.GenerateSasUrl(page.ImageBlobPath);
            result.AudioUrl = _blobUrlGenerator.GenerateSasUrl(page.AudioBlobPath);
            return result;
        }

        public async Task<PageDto?> UpdateAsync(string bookId, string sectionId, CreatePageDto dto)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null)
            {
                _logger.LogWarning("Page not found for update: BookId={BookId}, SectionId={SectionId}", bookId, sectionId);
                return null;
            }

            _mapper.Map(dto, existing);
            existing.Id = GeneratePageId(bookId, sectionId);
            existing.BookId = bookId;

            await _pageRepository.UpdateAsync(existing);
            _logger.LogInformation("Page updated: BookId={BookId}, SectionId={SectionId}", bookId, sectionId);

            var result = _mapper.Map<PageDto>(existing);
            result.ImageUrl = _blobUrlGenerator.GenerateSasUrl(existing.ImageBlobPath);
            result.AudioUrl = _blobUrlGenerator.GenerateSasUrl(existing.AudioBlobPath);
            return result;
        }

        public async Task<bool> DeleteAsync(string bookId, string sectionId)
        {
            var existing = await _pageRepository.GetBySectionIdAsync(bookId, sectionId);
            if (existing == null)
            {
                _logger.LogWarning("Page not found for deletion: BookId={BookId}, SectionId={SectionId}", bookId, sectionId);
                return false;
            }

            await _pageRepository.DeleteAsync(bookId, sectionId);
            _logger.LogInformation("Page deleted: BookId={BookId}, SectionId={SectionId}", bookId, sectionId);
            return true;
        }

        private static string GeneratePageId(string bookId, string sectionId) =>
            $"{bookId}_{sectionId}";
    }
}
