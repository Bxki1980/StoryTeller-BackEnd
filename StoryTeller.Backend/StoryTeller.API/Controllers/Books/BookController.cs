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
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Get all books with full details and their associated pages.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<BookDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<BookDto>>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(ApiResponse<List<BookDto>>.SuccessResponse(books));
        }

        /// <summary>
        /// Get a specific book by its ID, including pages.
        /// </summary>
        [HttpGet("{bookId}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetById(string bookId)
        {
            var book = await _bookService.GetByBookIdAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning("Book not found: {BookId}", bookId);
                return NotFound(ApiResponse<string>.Fail("Book not found"));
            }

            return Ok(ApiResponse<BookDto>.SuccessResponse(book));
        }

        /// <summary>
        /// Create a new book entry.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<BookDto>>> Create([FromBody] CreateBookDto dto)
        {
            var created = await _bookService.CreateAsync(dto);
            var response = ApiResponse<BookDto>.SuccessResponse(created);

            return CreatedAtAction(nameof(GetById), new { bookId = created.BookId }, response);
        }

        /// <summary>
        /// Update an existing book by its ID.
        /// </summary>
        [HttpPut("{bookId}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<BookDto>>> Update(string bookId, [FromBody] UpdateBookDto dto)
        {
            var updated = await _bookService.UpdateAsync(bookId, dto);
            if (updated == null)
            {
                _logger.LogWarning("Book not found for update: {BookId}", bookId);
                return NotFound(ApiResponse<string>.Fail("Book not found"));
            }

            return Ok(ApiResponse<BookDto>.SuccessResponse(updated));
        }

        /// <summary>
        /// Delete a book and all its pages.
        /// </summary>
        [HttpDelete("{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string bookId)
        {
            var success = await _bookService.DeleteAsync(bookId);
            if (!success)
            {
                _logger.LogWarning("Book not found for deletion: {BookId}", bookId);
                return NotFound(ApiResponse<string>.Fail("Book not found"));
            }

            return NoContent();
        }

        /// <summary>
        /// Get detailed info for a specific book by ID.
        /// </summary>
        [HttpGet("{bookId}/detail")]
        [ProducesResponseType(typeof(ApiResponse<BookDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<BookDetailDto>>> GetBookDetailById(string bookId)
        {
            var detail = await _bookService.GetBookDetailAsync(bookId);
            if (detail == null)
            {
                _logger.LogWarning("Book detail not found: {BookId}", bookId);
                return NotFound(ApiResponse<string>.Fail("Book not found"));
            }

            return Ok(ApiResponse<BookDetailDto>.SuccessResponse(detail));
        }

        /// <summary>
        /// Get all books with only cover image and metadata.
        /// </summary>
        [HttpGet("covers")]
        [ProducesResponseType(typeof(List<BookCoverDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookCoverDto>>> GetAllBookCovers()
        {
            try
            {
                var covers = await _bookService.GetAllBooksCoverAsync();
                return Ok(ApiResponse<List<BookCoverDto>>.SuccessResponse(covers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching book covers");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching book covers.");
            }
        }
    }
}
