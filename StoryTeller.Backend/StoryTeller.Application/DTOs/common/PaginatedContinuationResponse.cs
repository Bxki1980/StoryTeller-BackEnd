namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common
{
    public class PaginatedContinuationResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public string? ContinuationToken { get; set; }
    }
}
