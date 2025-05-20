namespace StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities
{
    using StoryTeller.Domain.Enums;

    
    public class User
    {
        public String Id { get; set; } = Guid.NewGuid().ToString();
        public String Emial { get; set; } = String.Empty;
        public String PasswordHash { get; set; } = String.Empty;
        public UserRole Role { get; set; } = UserRole.Free;
    }
}
