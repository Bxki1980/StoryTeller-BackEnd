namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth
{

    public class UserSignupDtos
    {
        public String Email { get; set; }
        public String Password { get; set; }
    }

    public class UserLoginDtos
    {
        public String Email { get; set; }
        public String Password { get; set; }
    }


    public class AuthResponseDtos
    {
        public String Token { get; set; }
        public String Email { get; set; }
        public String Role { get; set; }
    }

}