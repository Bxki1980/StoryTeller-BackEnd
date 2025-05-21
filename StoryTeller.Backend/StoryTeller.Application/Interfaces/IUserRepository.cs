using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces
{
    public interface IUserRepository
    {

        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
    }
}
