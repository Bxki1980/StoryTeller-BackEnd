namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' was not found.") { }
    }

}
