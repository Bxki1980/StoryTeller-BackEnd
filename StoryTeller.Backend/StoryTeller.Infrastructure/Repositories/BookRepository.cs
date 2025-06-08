using Microsoft.Azure.Cosmos;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Net;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly Container _container;

        public BookRepository(CosmosClient cosmosClient, IConfiguration config)
        {
            var databaseId = config["CosmosDb:DatabaseId"];
            var containerId = config["CosmosDb:BooksContainerId"];
            _container = cosmosClient.GetContainer("StoryTeller", "Books");
        }

        public async Task CreateAsync(Book book)
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
                await _container.DeleteItemAsync<Book>(book.Id, new PartitionKey(book.Id));
            }
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            using var iterator = _container.GetItemQueryIterator<Book>(query);
            var result = new List<Book>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<Book?> GetByBookIdAsync(string bookId)
        {
            var query = new QueryDefinition("SELECT * From c WHERE c.bookId = @bookId")
                .WithParameter("@bookId", bookId);

            using var iterator = _container.GetItemQueryIterator<Book>(query);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task UpdateAsync(Book book)
        {
            book.UpdatedAt = DateTime.UtcNow;
            await _container.UpsertItemAsync(book, new PartitionKey(book.Id));
        }
    }
}
