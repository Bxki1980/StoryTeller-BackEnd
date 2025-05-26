using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using System.Security.Claims;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces
{
    public interface IGoogleAuthService
    {
            Task<AuthResponseDto> HandleGoogleCallbackAsync(ClaimsPrincipal principal);
            Task<AuthResponseDto> HandleGoogleSignInTokenAsync(string idToken);
    }
}
