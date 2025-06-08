using AutoMapper;
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

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto?> GetByBookIdAsync(string bookId)
        {
            var book = await _bookRepository.GetByBookIdAsync(bookId);
            return book == null ? null : _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
            var book = _mapper.Map<StoryTeller.Domain.Entities.Book>(dto);
            book.Id = Guid.NewGuid().ToString();
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = book.CreatedAt;
            book.Version = "1.0.0";

            await _bookRepository.CreateAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto?> UpdateAsync(string bookId, UpdateBookDto dto)
        {
            var existing = await _bookRepository.GetByBookIdAsync(bookId);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync(existing);
            return _mapper.Map<BookDto>(existing);
        }

        public async Task<bool> DeleteAsync(string bookId)
        {
            var book = await _bookRepository.GetByBookIdAsync(bookId);
            if (book == null)
                return false;

            await _bookRepository.DeleteAsync(bookId);
            return true;
        }
    }
}
