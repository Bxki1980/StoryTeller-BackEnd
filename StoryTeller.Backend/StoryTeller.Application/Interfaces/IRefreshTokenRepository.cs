using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task InvalidateAsync(string token);
    }
}
