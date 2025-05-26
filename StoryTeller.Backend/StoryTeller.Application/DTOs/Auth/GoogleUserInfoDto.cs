namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth
{
    public class GoogleUserInfoDto
    {
        public string Email { get; set; }
        public string NameIdentifier { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string Locale { get; set; }
    }
}
