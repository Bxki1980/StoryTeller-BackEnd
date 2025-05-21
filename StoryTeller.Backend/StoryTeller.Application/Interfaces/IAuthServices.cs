using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDto> RegisterAsync(UserSignupDto dto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
    }
}
