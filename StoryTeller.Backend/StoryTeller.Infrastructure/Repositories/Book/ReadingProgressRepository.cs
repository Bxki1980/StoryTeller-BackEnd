using AutoMapper;
using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Net;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories.Book
{
    public class ReadingProgressService : IReadingProgressService
    {
        private readonly IReadingProgressRepository _repo;
        private readonly IMapper _mapper;

        public ReadingProgressService(IReadingProgressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ReadingProgressDto?> GetProgressAsync(string userId, string bookId)
        {
            var entity = await _repo.GetAsync(userId, bookId);
            return entity is null ? null : _mapper.Map<ReadingProgressDto>(entity);
        }

        public async Task SaveProgressAsync(ReadingProgressDto dto)
        {
            var entity = _mapper.Map<ReadingProgress>(dto);
            await _repo.UpsertAsync(entity);
        }
    }


}
