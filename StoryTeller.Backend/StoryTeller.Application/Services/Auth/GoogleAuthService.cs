using Google.Apis.Auth;
using System.Security.Claims;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using Microsoft.Extensions.Options;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IGoogleClaimsParser _claimsParser;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly TokenService _tokenService;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly ILoggerManager _logger;
    private readonly JwtSettings _jwtSettings;

    public GoogleAuthService(
        IGoogleClaimsParser claimsParser,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepo,
        TokenService tokenService,
        JwtTokenGenerator tokenGenerator,
        ILoggerManager logger,
        IOptions<JwtSettings> options
        )
    {
        _claimsParser = claimsParser;
        _userRepository = userRepository;
        _refreshTokenRepo = refreshTokenRepo;
        _tokenService = tokenService;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
        _jwtSettings = options.Value;
    }

    public async Task<AuthResponseDto> HandleGoogleCallbackAsync(ClaimsPrincipal principal)
    {
        var dto = _claimsParser.Parse(principal);
        var user = await EnsureGoogleUserAsync(dto);
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> HandleGoogleSignInTokenAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        if (payload == null)
            throw new Exception("Invalid Google ID token");

        var dto = new GoogleUserInfoDto
        {
            Email = payload.Email,
            NameIdentifier = payload.Subject,
            GivenName = payload.GivenName,
            FamilyName = payload.FamilyName,
            Picture = payload.Picture,
            Locale = payload.Locale
        };

        var user = await EnsureGoogleUserAsync(dto);
        return await GenerateAuthResponseAsync(user);
    }

    private async Task<User> EnsureGoogleUserAsync(GoogleUserInfoDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            user = new User
            {
                Email = dto.Email,
                ExternalProvider = "Google",
                ExternalId = dto.NameIdentifier,
                FirstName = dto.GivenName,
                LastName = dto.FamilyName,
                PictureUrl = dto.Picture,
                Locale = dto.Locale,
                Role = StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Enums.UserRole.Free
            };

            await _userRepository.CreateAsync(user);
            _logger.LogInfo($"Created new Google user: {user.Email}");
        }
        else if (string.IsNullOrEmpty(user.ExternalProvider))
        {
            user.ExternalProvider = "Google";
            user.ExternalId = dto.NameIdentifier;
            await _userRepository.UpdateAsync(user);
            _logger.LogInfo($"Linked existing user {user.Email} to Google");
        }

        return user;
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        var jwt = _tokenGenerator.GenerateToken(user);

        var refreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        refreshToken.Token = _tokenService.HashToken(refreshToken.Token);
        await _refreshTokenRepo.CreateAsync(refreshToken);

        return new AuthResponseDto
        {
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = jwt,
            RefreshToken = refreshToken.Token
        };
    }
}
