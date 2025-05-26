namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions
{
    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message)
            : base(message, 401) { }
    }

}
