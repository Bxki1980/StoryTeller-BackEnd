using Microsoft.Azure.Cosmos;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Database
{
    public class CosmosDbFactory
    {
        public static CosmosClient CreateClient(IConfiguration config)
        {
            var connectionString = config["Cosmos:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing Cosmos:ConnectionString configuration");

            return new CosmosClient(connectionString);
        }
    }
}
