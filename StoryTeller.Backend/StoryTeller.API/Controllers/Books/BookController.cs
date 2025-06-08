using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services;

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
            return Ok(books);
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetById(string bookId)
        {
            var book = await _bookService.GetByBookIdAsync(bookId);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            var created = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { bookId = created.BookId }, created);
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
            return success ? NoContent() : NotFound();
        }
    }
}
