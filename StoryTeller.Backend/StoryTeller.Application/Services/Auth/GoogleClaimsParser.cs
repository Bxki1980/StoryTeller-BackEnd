using System.Security.Claims;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Services.Auth
{
    public class GoogleClaimsParser : IGoogleClaimsParser
    {
        public GoogleUserInfoDto Parse(ClaimsPrincipal principal)
        {
            return new GoogleUserInfoDto
            {
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                NameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                GivenName = principal.FindFirst(ClaimTypes.GivenName)?.Value,
                FamilyName = principal.FindFirst(ClaimTypes.Surname)?.Value,
                Picture = principal.FindFirst("urn:google:picture")?.Value,
                Locale = principal.FindFirst("urn:google:locale")?.Value
            };
        }
    }
}
