using System.Security.Claims;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services
{
    public interface IGoogleClaimsParser
    {
        GoogleUserInfoDto Parse(ClaimsPrincipal principal);
    }
}
