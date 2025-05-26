namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    public class UserWithToken
    {
        public User User { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
