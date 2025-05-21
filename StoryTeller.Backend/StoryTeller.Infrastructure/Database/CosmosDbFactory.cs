using Microsoft.Azure.Cosmos;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Database
{
    public class CosmosDbFactory
    {
        public static CosmosClient CreateClient(IConfiguration config)
        {
            var connectionString = config["Cosmos:ConnectionString"];
            return new CosmosClient(connectionString);
        }
    }
}
