using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitUserWithTokenAsync(User user, RefreshToken token);
    }
}
