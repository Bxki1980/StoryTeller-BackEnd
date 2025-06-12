using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.BlobStorage
{
    public class BlobUrlGenerator : IBlobUrlGenerator
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly StorageSharedKeyCredential _credential;

        public BlobUrlGenerator(IConfiguration config)
        {
            var accountName = config["AzureStorage:AccountName"];
            var accountKey = config["AzureStorage:AccountKey"];
            var containerName = config["AzureStorage:ContainerName"];

            if (string.IsNullOrWhiteSpace(accountName))
                throw new InvalidOperationException("AzureStorage:AccountName is missing or empty.");

            if (string.IsNullOrWhiteSpace(accountKey))
                throw new InvalidOperationException("AzureStorage:AccountKey is missing or empty.");

            if (string.IsNullOrWhiteSpace(containerName))
                throw new InvalidOperationException("AzureStorage:ContainerName is missing or empty.");

            _containerName = containerName;

            _credential = new StorageSharedKeyCredential(accountName, accountKey);
            _blobServiceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                _credential);
        }

        public string GenerateSasUrl(string blobPath, TimeSpan? expiry = null)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = container.GetBlobClient(blobPath);

            var expiresOn = DateTimeOffset.UtcNow.Add(expiry ?? TimeSpan.FromHours(1));

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobPath,
                Resource = "b",
                ExpiresOn = expiresOn
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(_credential).ToString();
            var uri = $"{blobClient.Uri}?{sasToken}";

            return uri;
        }
    }
}
