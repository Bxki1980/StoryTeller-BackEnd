using Newtonsoft.Json;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common
{
    /// <summary>
    /// Parameters for continuation-based pagination in Cosmos DB.
    /// </summary>
    public class PageQueryParameters
    {
        /// <summary>
        /// Continuation token from the previous page (opaque string from Cosmos DB).
        /// </summary>
        public string? ContinuationToken { get; set; }

        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        /// <summary>
        /// The number of items to return per page (max 50).
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 10 : Math.Min(value, MaxPageSize);
        }

        /// <summary>
        /// Page number used only for traditional paging (ignored in Cosmos continuation).
        /// </summary>
        [JsonIgnore]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items to skip (used only in offset-based paging, not Cosmos).
        /// </summary>
        [JsonIgnore]
        public int Skip => (PageNumber - 1) * PageSize;
    }
}
