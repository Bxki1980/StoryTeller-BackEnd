namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common
{
    public class PaginatedContinuationResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        /// Token to fetch the next page in a continuation-based query.
        public string? ContinuationToken { get; set; } 
    }
}
