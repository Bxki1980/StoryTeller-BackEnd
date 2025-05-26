using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using User = StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities.User;

public class CosmosUnitOfWork : IUnitOfWork
{
    private readonly Container _container;

    public CosmosUnitOfWork(CosmosClient client, IConfiguration config)
    {
        var dbName = config["Cosmos:Database"];
        var containerName = config["Cosmos:UserContainer"];
        _container = client.GetContainer(dbName, containerName);
    }

    public async Task<bool> CommitUserWithTokenAsync(User user, RefreshToken token)
    {
        var partitionKey = new PartitionKey(user.Email);

        var batch = _container.CreateTransactionalBatch(partitionKey)
            .CreateItem(user)
            .CreateItem(token);

        var response = await batch.ExecuteAsync();
        return response.IsSuccessStatusCode;
    }
}
