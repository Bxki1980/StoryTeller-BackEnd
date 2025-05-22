using Microsoft.Azure.Cosmos;

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


            try
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

                }

                return null;
            }
            catch (CosmosException ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }
        public async Task CreateAsync(User user)
        {
            try
            {
                await _container.CreateItemAsync(user, new PartitionKey(user.Id));
            }
            catch (CosmosException ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public async Task UpdateAsync(User user)
        {
            try 
            {
                await _container.ReplaceItemAsync(user, user.Id, new PartitionKey(user.Id));
            }
            catch (CosmosException ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public async Task DeleteAsync(string userId, string partitionKey)
        {
            try
            {
                await _container.DeleteItemAsync<User>(userId, new PartitionKey(partitionKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete failed: {ex.Message}");
            }
        }
    }
}


