using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<List<BookCoverDto>> GetAllBooksCoverAsync();
        Task<BookDetailDto> GetBookDetailAsync(string bookId);
        Task<BookDto?> GetByBookIdAsync(string bookId);
        Task<BookDto> CreateAsync(CreateBookDto dto);
        Task<BookDto?> UpdateAsync(string bookId, UpdateBookDto dto);
        Task<bool> DeleteAsync(string bookId);
    }
}
