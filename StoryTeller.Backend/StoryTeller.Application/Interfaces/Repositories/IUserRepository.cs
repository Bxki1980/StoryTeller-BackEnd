using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {

        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string userId, string partitionKey);
    }
}
