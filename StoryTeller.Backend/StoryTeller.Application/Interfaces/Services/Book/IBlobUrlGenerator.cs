namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book
{
    public interface IBlobUrlGenerator
    {
        string GenerateSasUrl(string blobPath, TimeSpan? expiry = null);
    }
}
