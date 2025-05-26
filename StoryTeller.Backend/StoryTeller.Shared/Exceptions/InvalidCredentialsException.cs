namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions
{
    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException() : base("Invalid email or password.") { }
    }

}
