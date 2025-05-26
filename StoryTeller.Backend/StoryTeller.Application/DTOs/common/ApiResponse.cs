namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data) => new() { Success = true, Data = data };
        public static ApiResponse<T> Fail(params string[] errors) => new() { Success = false, Errors = errors.ToList() };
    }
}
