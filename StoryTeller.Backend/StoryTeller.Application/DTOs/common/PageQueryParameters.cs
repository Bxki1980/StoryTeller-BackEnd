using Newtonsoft.Json;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common
{
    public class PageQueryParameters
    {
        public string? ContinuationToken { get; set; }

        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, MaxPageSize);
        }

        [JsonIgnore] // if you're only using continuation-based paging
        public int PageNumber { get; set; } = 1;

        [JsonIgnore]
        public int Skip => (PageNumber - 1) * PageSize;
    }

}
