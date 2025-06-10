using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(ApiResponse<List<BookDto>>.SuccessResponse(books));
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetById(string bookId)
        {
            var book = await _bookService.GetByBookIdAsync(bookId);
            if (book == null)
                return NotFound();

            return Ok(ApiResponse<BookDto>.SuccessResponse(book));
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            var created = await _bookService.CreateAsync(dto);
            var response = ApiResponse<BookDto>.SuccessResponse(created);
            return CreatedAtAction(nameof(GetById), new { bookId = created.BookId }, response);
        }

        [HttpPut("{bookId}")]
        public async Task<ActionResult<BookDto>> Update(string bookId, [FromBody] UpdateBookDto dto)
        {
            var updated = await _bookService.UpdateAsync(bookId, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> Delete(string bookId)
        {
            var success = await _bookService.DeleteAsync(bookId);
            if (!success)
                return NotFound(new ProblemDetails { Title = "Book not found", Status = 404 });

            return NoContent(); // 204
        }

        [HttpGet("covers")]
        public async Task<ActionResult<ApiResponse<List<BookCoverDto>>>> GetAllCovers()
        {
            var covers = await _bookService.GetAllBooksCoverAsync();
            return Ok(ApiResponse<List<BookCoverDto>>.SuccessResponse(covers));
        }
    }
}
