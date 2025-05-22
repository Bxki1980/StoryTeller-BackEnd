using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly Container _container;


        public RefreshTokenRepository(IConfiguration config)
        {
            var client = new CosmosClient(config["Cosmos:ConnectionString"]);
            var database = client.GetDatabase(config["Cosmos:DatabaseName"]);
            _container = database.GetContainer(config["Cosmos:UserContainer"]);
        }

        public async Task CreateAsync(RefreshToken token)
        {
           await _container.CreateItemAsync(token, new PartitionKey(token.UserId));
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.token = @token")
                .WithParameter("@token", token);

            var iterator = _container.GetItemQueryIterator<RefreshToken>(query);
            while(iterator.HasMoreResults)
            {
                foreach (var result in await iterator.ReadNextAsync())
                    return result;
            }

            return null;
        }

        public async Task InvalidateAsync(string token)
        {
            var existing = await GetByTokenAsync(token);
            if (existing != null)
            {
                existing.Revoked = true;
                await _container.ReplaceItemAsync(existing, existing.Id, new PartitionKey(existing.UserId));
            }
        }
    }
}
