using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book
{
    public class BlobUrlGenerator : IBlobUrlGenerator
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly StorageSharedKeyCredential _credential;

        public BlobUrlGenerator(IConfiguration config)
        {
            var accountName = config["Azure:BlobStorage:AccountName"];
            var accountKey = config["Azure:BlobStorage:AccountKey"];
            _containerName = config["Azure:BlobStorage:ContainerName"]!;

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
