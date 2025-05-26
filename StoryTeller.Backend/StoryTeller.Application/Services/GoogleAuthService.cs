using AutoMapper;
using Google.Apis.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Enums;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using System.Security.Claims;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly TokenService _tokenService;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;


    public GoogleAuthService(
        IUserRepository userRepo,
        IRefreshTokenRepository refreshRepo,
        TokenService tokenService,
        JwtTokenGenerator tokenGenerator,
        IMapper mapper,
        IConfiguration config)
    {
        _userRepository = userRepo;
        _refreshTokenRepo = refreshRepo;
        _tokenService = tokenService;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
        _refreshTokenExpiryDays = int.TryParse(config["Jwt:RefreshTokenExpiryDays"], out var days) ? days : 7;
    }

    public async Task<AuthResponseDto> HandleGoogleCallbackAsync(ClaimsPrincipal principal)
    {
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var nameId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nameId))
            throw new InvalidOperationException("Missing Google user info");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                Email = email,
                Role = UserRole.Free,
                ExternalProvider = "Google",
                ExternalId = nameId,
                FirstName = principal.FindFirst(ClaimTypes.GivenName)?.Value,
                LastName = principal.FindFirst(ClaimTypes.Surname)?.Value,
                PictureUrl = principal.FindFirst("urn:google:picture")?.Value,
                Locale = principal.FindFirst("urn:google:locale")?.Value
            };
            await _userRepository.CreateAsync(user);
        }
        else if (string.IsNullOrEmpty(user.ExternalProvider))
        {
            user.ExternalProvider = "Google";
            user.ExternalId = nameId;
            await _userRepository.UpdateAsync(user);
        }

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> HandleGoogleSignInTokenAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        var email = payload.Email;

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            var dto = new GoogleUserInfoDto
            {
                Email = payload.Email,
                NameIdentifier = payload.Subject,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                Picture = payload.Picture,
                Locale = payload.Locale
            };

            user = _mapper.Map<User>(dto);
            await _userRepository.CreateAsync(user);
        }
        else if (string.IsNullOrEmpty(user.ExternalProvider))
        {
            user.ExternalProvider = "Google";
            user.ExternalId = payload.Subject;
            await _userRepository.UpdateAsync(user);
        }

        return await GenerateAuthResponse(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var token = _tokenGenerator.GenerateToken(user);
        var refresh = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
        };

        refresh.Token = _tokenService.HashToken(refresh.Token);
        await _refreshTokenRepo.CreateAsync(refresh);

        return new AuthResponseDto
        {
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refresh.Token
        };
    }
}
