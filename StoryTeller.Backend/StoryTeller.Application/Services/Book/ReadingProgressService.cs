using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book
{
    public class ReadingProgressService
    {
        private readonly IReadingProgressRepository _repo;

        public ReadingProgressService(IReadingProgressRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReadingProgressDto?> GetAsync(string userId, string bookId)
        {
            var progress = await _repo.GetAsync(userId, bookId);
            return progress is null ? null : new ReadingProgressDto
            {
                UserId = progress.UserId,
                BookId = progress.BookId,
                SectionIndex = progress.SectionIndex
            };
        }

        public async Task SaveAsync(ReadingProgressDto dto)
        {
            var progress = new ReadingProgress
            {
                UserId = dto.UserId,
                BookId = dto.BookId,
                SectionIndex = dto.SectionIndex
            };
            await _repo.UpsertAsync(progress);
        }
    }

}
