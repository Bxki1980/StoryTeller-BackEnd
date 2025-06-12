namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.common
{
    public class PageQueryParameters
    {
        public string? ContinuationToken { get; set; }
        public int PageNumber { get; set; } = 1; // Default page

        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int Skip => (PageNumber - 1) * PageSize;
    }

}
