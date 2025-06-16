using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Net;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories.Book
{
    public class PageRepository : IPageRepository
    {
        private readonly Container _container;
        private readonly IMapper _mapper;


        public PageRepository(CosmosClient cosmosClient, IConfiguration config, IMapper mapper )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var databaseId = config["Cosmos:DatabaseName"]
                ?? throw new InvalidOperationException("Missing Cosmos:DatabaseName in configuration");

            var containerId = config["Cosmos:PagesContainerId"]
                ?? throw new InvalidOperationException("Missing Cosmos:PagesContainerId in configuration");

            _container = cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<List<Page>> GetPagesByBookIdRawAsync(string bookId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.bookId = @bookId")
                .WithParameter("@bookId", bookId);

            using var iterator = _container.GetItemQueryIterator<Page>(query);
            var pages = new List<Page>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                pages.AddRange(response);
            }

            return pages;
        }

        public async Task<PaginatedContinuationResponse<Page>> GetPaginatedPagesByBookIdAsync(
     string bookId,
     PageQueryParameters queryParams)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("BookId must be provided.", nameof(bookId));

            var query = new QueryDefinition("SELECT * FROM c WHERE c.bookId = @bookId")
                .WithParameter("@bookId", bookId);

            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(bookId),
                MaxItemCount = queryParams.PageSize
            };

            // ✅ Use raw string as continuationToken
            var iterator = _container.GetItemQueryIterator<Page>(
                query,
                queryParams.ContinuationToken,
                requestOptions
            );

            var response = await iterator.ReadNextAsync();

            return new PaginatedContinuationResponse<Page>
            {
                Data = response.Resource,
                ContinuationToken = response.ContinuationToken
            };
        }


        public async Task<Page?> GetBySectionIdAsync(string bookId, string sectionId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.bookId = @bookId AND c.sectionId = @sectionId")
                .WithParameter("@bookId", bookId)
                .WithParameter("@sectionId", sectionId);

            using var iterator = _container.GetItemQueryIterator<Page>(query);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task CreateAsync(Page page)
        {
            page.CreatedAt = DateTime.UtcNow;
            page.Id = GeneratePageId(page.BookId, page.SectionId);

            await _container.CreateItemAsync(page, new PartitionKey(page.BookId));
        }

        public async Task UpdateAsync(Page page)
        {
            page.Id = GeneratePageId(page.BookId, page.SectionId);

            await _container.UpsertItemAsync(page, new PartitionKey(page.BookId));
        }

        public async Task DeleteAsync(string bookId, string sectionId)
        {
            var id = GeneratePageId(bookId, sectionId);
            await _container.DeleteItemAsync<Page>(id, new PartitionKey(bookId));
        }


        public async Task CreateManyAsync(List<Page> pages)
        {
            var tasks = pages.Select(p =>
                _container.CreateItemAsync(p, new PartitionKey(p.BookId))
            );
            await Task.WhenAll(tasks);
        }


        private static string GeneratePageId(string bookId, string sectionId) =>
            $"{bookId}_{sectionId}";
    }

}
