using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Net;

public class ReadingProgressRepository : IReadingProgressRepository
{
    private readonly Container _container;

    public ReadingProgressRepository(CosmosClient client, IConfiguration config)
    {
        _container = client.GetContainer("YourDbName", "ReadingProgress");
    }

    public async Task<ReadingProgress?> GetAsync(string userId, string bookId)
    {
        try
        {
            var response = await _container.ReadItemAsync<ReadingProgress>(
                id: $"{userId}:{bookId}", partitionKey: new PartitionKey(userId));
            return response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertAsync(ReadingProgress progress)
    {
        await _container.UpsertItemAsync(progress, new PartitionKey(progress.UserId));
    }
}
