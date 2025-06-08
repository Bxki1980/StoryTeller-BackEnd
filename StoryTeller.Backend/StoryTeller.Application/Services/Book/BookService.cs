using AutoMapper;
using Microsoft.Extensions.Logging;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IBlobUrlGenerator _blobUrlGenerator;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, IMapper mapper, IBlobUrlGenerator blobUrlGenerator, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _blobUrlGenerator = blobUrlGenerator;
            _logger = logger;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            var dtos = books.Select(book =>
            {
                var dto = _mapper.Map<BookDto>(book);
                dto.CoverImageUrl = _blobUrlGenerator.GenerateSasUrl(book.CoverImageBlobPath);
                return dto;
            }).ToList();

            _logger.LogInformation("Fetched {Count} books", dtos.Count);
            return dtos;
        }

        public async Task<BookDto?> GetByBookIdAsync(string bookId)
        {
            var book = await _bookRepository.GetByBookIdAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning("Book not found: {BookId}", bookId);
                return null;
            }

            var dto = _mapper.Map<BookDto>(book);
            dto.CoverImageUrl = _blobUrlGenerator.GenerateSasUrl(book.CoverImageBlobPath);
            return dto;
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
            var book = _mapper.Map<StoryTeller.Domain.Entities.Book>(dto);
            book.Id = Guid.NewGuid().ToString();
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = book.CreatedAt;
            book.Version = "1.0.0";

            await _bookRepository.CreateAsync(book);
            _logger.LogInformation("Book created successfully: {BookId}", book.BookId);

            var result = _mapper.Map<BookDto>(book);
            result.CoverImageUrl = _blobUrlGenerator.GenerateSasUrl(book.CoverImageBlobPath);
            return result;
        }

        public async Task<BookDto?> UpdateAsync(string bookId, UpdateBookDto dto)
        {
            var existing = await _bookRepository.GetByBookIdAsync(bookId);
            if (existing == null)
            {
                _logger.LogWarning("Book not found for update: {BookId}", bookId);
                return null;
            }

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync(existing);
            _logger.LogInformation("Book updated: {BookId}", bookId);

            var result = _mapper.Map<BookDto>(existing);
            result.CoverImageUrl = _blobUrlGenerator.GenerateSasUrl(existing.CoverImageBlobPath);
            return result;
        }

        public async Task<bool> DeleteAsync(string bookId)
        {
            var book = await _bookRepository.GetByBookIdAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning("Book not found for deletion: {BookId}", bookId);
                return false;
            }

            await _bookRepository.DeleteAsync(bookId);
            _logger.LogInformation("Book deleted: {BookId}", bookId);
            return true;
        }
    }
}
