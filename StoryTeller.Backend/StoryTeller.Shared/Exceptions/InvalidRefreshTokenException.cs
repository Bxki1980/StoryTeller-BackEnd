namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions
{
    public class InvalidRefreshTokenException : AppException
    {
        public InvalidRefreshTokenException() : base("Invalid or expired refresh token.") { }
    }
}
