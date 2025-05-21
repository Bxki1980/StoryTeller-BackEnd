using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly Container _container;

        public UserRepository(IConfiguration config)
        {
            var client = new CosmosClient(config["Cosmos:ConnectionString"]);
            var database = client.GetDatabase(config["Cosmos:DatabaseName"]);
            _container = database.GetContainer(config["Cosmos:UserContainer"]);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var query = new QueryDefinition("SELECT * from c where c.email = @email")
                .WithParameter("@email", email);


            using var iterator = _container.GetItemQueryIterator<User>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var user in await iterator.ReadNextAsync())
                {
                    return user;
                }
                return null;
            }
        }
        public async Task CreateAsync(User user)
        {
            await _container.CreateItemAsync(user, new PartitionKey(user.BookId));
        }
    }
}


