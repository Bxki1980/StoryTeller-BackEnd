namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth
{

    public class UserSignupDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }

    public class RefreshRequestDto
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}