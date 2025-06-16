using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly ILogger<PageController> _logger;

        public PageController(IPageService pageService, ILogger<PageController> logger)
        {
            _pageService = pageService;
            _logger = logger;
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<List<PageDto>>> GetPagesByBookId(string bookId)
        {

            var pages = await _pageService.GetPagesByBookIdAsync(bookId);
            return Ok(pages);
        }

        [HttpGet("book/{bookId}/section/{sectionId}")]
        public async Task<ActionResult<PageDto>> GetBySectionId(string bookId, string sectionId)
        {
            var page = await _pageService.GetBySectionIdAsync(bookId, sectionId);
            if (page == null)
                return NotFound();

            return Ok(page);
        }

        [HttpPost("book/{bookId}")]
        public async Task<ActionResult<PageDto>> Create(string bookId, [FromBody] CreatePageDto dto)
        {
            var created = await _pageService.CreateAsync(bookId, dto);
            return CreatedAtAction(nameof(GetBySectionId),
                new { bookId = created.BookId, sectionId = dto.SectionId }, created);
        }

        [HttpPut("book/{bookId}/section/{sectionId}")]
        public async Task<ActionResult<PageDto>> Update(string bookId, string sectionId, [FromBody] CreatePageDto dto)
        {
            var updated = await _pageService.UpdateAsync(bookId, sectionId, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("book/{bookId}/section/{sectionId}")]
        public async Task<IActionResult> Delete(string bookId, string sectionId)
        {
            var success = await _pageService.DeleteAsync(bookId, sectionId);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("book/{bookId}/batch")]
        public async Task<ActionResult<List<PageDto>>> CreateBatch(string bookId, [FromBody] List<CreatePageDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                return BadRequest("Pages cannot be empty.");

            var createdPages = await _pageService.CreateBatchAsync(bookId, dtos);
            return Ok(ApiResponse<List<PageDto>>.SuccessResponse(createdPages));

        }


        /// <summary>
        /// Retrieves a paginated list of pages for a specific book using continuation token.
        /// </summary>
        /// <param name="bookId">The ID of the book (also used as partition key).</param>
        /// <param name="queryParams">Pagination parameters including page size and continuation token.</param>
        /// <returns>A paginated response containing a list of PageDto objects and a continuation token.</returns>
        [HttpGet("book/{bookId}/pages")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedContinuationResponse<PageDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaginatedContinuationResponse<PageDto>>>> GetPaginatedPages(
            string bookId,
            [FromQuery] PageQueryParameters queryParams)
        {
            if (string.IsNullOrWhiteSpace(bookId))
            {
                _logger.LogWarning("❌ Book ID was null or empty in GetPaginatedPages.");
                return BadRequest(ApiResponse<string>.Fail("Book ID is required."));
            }

            _logger.LogInformation("📥 Received request to get paginated pages for BookId: {BookId}", bookId);

            var result = await _pageService.GetPaginatedPagesByBookIdAsync(bookId, queryParams);

            _logger.LogInformation("📤 Successfully returned {Count} pages. ContinuationToken: {Token}",
                result.Data.Count(), result.ContinuationToken ?? "<null>");

            return Ok(ApiResponse<PaginatedContinuationResponse<PageDto>>.SuccessResponse(result));
        }


    }
}
