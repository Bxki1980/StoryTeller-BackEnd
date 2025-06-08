using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Auth
{
    public interface IAuthServices
    {
        Task<AuthResponseDto> RegisterAsync(UserSignupDto dto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto);
        Task LogoutAsync(RefreshRequestDto dto);

    }
}
