using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
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
            return Ok(createdPages);
        }
    }
}
