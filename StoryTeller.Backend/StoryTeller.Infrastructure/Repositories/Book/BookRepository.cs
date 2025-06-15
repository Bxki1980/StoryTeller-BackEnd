using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Net;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories.Book
{
    public class BookRepository : IBookRepository
    {
        private readonly Container _container;

        public BookRepository(CosmosClient cosmosClient, IConfiguration config)
        {
            var databaseId = config["Cosmos:DatabaseName"]
                ?? throw new InvalidOperationException("Missing Cosmos:DatabaseName in configuration");

            var containerId = config["Cosmos:BooksContainerId"]
                ?? throw new InvalidOperationException("Missing Cosmos:BooksContainerId in configuration");

            _container = cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task CreateAsync(StoryTeller.Domain.Entities.Book book)
        {
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;
            await _container.CreateItemAsync(book, new PartitionKey(book.Id));
        }

        public async Task DeleteAsync(string bookId)
        {
            var book = await GetByBookIdAsync(bookId);
            if (book is not null)
            {
                await _container.DeleteItemAsync<StoryTeller.Domain.Entities.Book>(book.Id, new PartitionKey(book.Id));
            }
        }

        public async Task<List<StoryTeller.Domain.Entities.Book>> GetAllAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            using var iterator = _container.GetItemQueryIterator<StoryTeller.Domain.Entities.Book>(query);
            var result = new List<StoryTeller.Domain.Entities.Book>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<StoryTeller.Domain.Entities.Book?> GetByBookIdAsync(string bookId)
        {
            var query = new QueryDefinition("SELECT * From c WHERE c.bookId = @bookId")
                .WithParameter("@bookId", bookId);

            using var iterator = _container.GetItemQueryIterator<StoryTeller.Domain.Entities.Book>(query);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task UpdateAsync(StoryTeller.Domain.Entities.Book book)
        {
            book.UpdatedAt = DateTime.UtcNow;
            await _container.UpsertItemAsync(book, new PartitionKey(book.Id));
        }
    }
}
