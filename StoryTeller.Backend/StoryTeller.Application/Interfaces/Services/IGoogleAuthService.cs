using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using System.Security.Claims;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services
{
    public interface IGoogleAuthService
    {
            Task<AuthResponseDto> HandleGoogleCallbackAsync(ClaimsPrincipal principal);
            Task<AuthResponseDto> HandleGoogleSignInTokenAsync(string idToken);
    }
}
