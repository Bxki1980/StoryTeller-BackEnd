namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions
{
    public class UserAlreadyExistsException : AppException
    {
        public UserAlreadyExistsException(string email)
            : base($"User with email '{email}' already exists.") { }
    }
}
