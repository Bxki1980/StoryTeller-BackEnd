namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth
{

    public class UserSignupDto
    {
        public String Email { get; set; }
        public String Password { get; set; }
    }

    public class UserLoginDto
    {
        public String Email { get; set; }
        public String Password { get; set; }
    }


    public class AuthResponseDto
    {
        public String Token { get; set; }
        public String Email { get; set; }
        public String Role { get; set; }
    }

}