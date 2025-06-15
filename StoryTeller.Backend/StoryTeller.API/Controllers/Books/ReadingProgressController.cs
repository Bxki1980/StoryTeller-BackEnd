using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadingProgressController : ControllerBase
    {
        private readonly IReadingProgressService _service;

        public ReadingProgressController(IReadingProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProgress([FromQuery] string userId, [FromQuery] string bookId)
        {
            var result = await _service.GetProgressAsync(userId, bookId);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProgress([FromBody] ReadingProgressDto dto)
        {
            await _service.SaveProgressAsync(dto);
            return NoContent();
        }
    }

}
