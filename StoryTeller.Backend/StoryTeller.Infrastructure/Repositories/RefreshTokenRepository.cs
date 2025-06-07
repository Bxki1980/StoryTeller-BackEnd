using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories;
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
            _container = database.GetContainer(config["Cosmos:RefreshTokenContainer"]);
        }

        public async Task CreateAsync(RefreshToken token)
        {
           await _container.CreateItemAsync(token, new PartitionKey(token.Id));
        }

        public async Task<RefreshToken> GetByTokenAsync(string hashedToken)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.token = @token")
                .WithParameter("@token", hashedToken);

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
