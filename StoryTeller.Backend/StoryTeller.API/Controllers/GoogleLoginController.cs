using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Enums;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using System.Security.Claims;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GoogleLoginController : ControllerBase
    {


        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly TokenService _tokenService;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly int _refreshTokenExpiryDays;


        public GoogleLoginController(IUserRepository userRepo, ILoggerManager logger, IMapper mapper)
        {
            _userRepository = userRepo;
            _logger = logger;
            _mapper = mapper;
            _refreshTokenExpiryDays = int.TryParse(Environment.GetEnvironmentVariable("Jwt:RefreshTokenExpiryDays"), out var days) ? days : 7;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(GoogleLogin))
                };

                _logger.LogInfo("Redirecting to Google login");
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Google Login failed {ex.Message}");
                return BadRequest(new { message = "Login failed" });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {

            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (!result.Succeeded)
                {
                    _logger.LogError("Google Login failed: Authentication result not successful");
                    return Unauthorized("Google Login failed");
                }

                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var nameId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var name = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nameId))
                {
                    _logger.LogError("Missing user info from Google");
                    return BadRequest("Missing user info from Google");
                }

                var firstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
                var pictureUrl = result.Principal.FindFirst("urn:google:picture")?.Value;
                var locale = result.Principal.FindFirst("urn:google:locale")?.Value;



                //Check or create user
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        Role = UserRole.Free,
                        ExternalProvider = "Google",
                        ExternalId = nameId,
                        FirstName = firstName,
                        LastName = lastName,
                        PictureUrl = pictureUrl,
                        Locale = locale
                    };

                    await _userRepository.CreateAsync(user);
                    _logger.LogInfo($"New user created: {email} with Google login");
                }
                else if (string.IsNullOrEmpty(user.ExternalProvider))
                {
                    user.ExternalProvider = "Google";
                    user.ExternalId = nameId;
                    await _userRepository.UpdateAsync(user);
                    _logger.LogInfo($"User {email} linked to Google account");
                }

                // Issure JWT
                var jwt = _tokenGenerator.GenerateToken(user);

                // Create refresh token
                var refreshToken = new RefreshToken
                {
                    Token = _tokenService.GenerateRefreshToken(),
                    UserId = user.Id,
                    Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
                };

                refreshToken.Token = _tokenService.HashToken(refreshToken.Token);
                await _refreshTokenRepo.CreateAsync(refreshToken);
                _logger.LogInfo($"User {email} logged in with Google");

                return Ok(new AuthResponseDto
                {
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Token = jwt,
                    RefreshToken = refreshToken.Token // return hashed or raw token based on your design
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Google callback failed: {ex.Message}");
                return BadRequest(new { message = "Google login failed" });
            }

        }

        [HttpPost("google-signin-token")]
        public async Task<IActionResult> GoogleSignInWithToken([FromBody] string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                if (payload == null)
                {
                    _logger.LogError("Invalid Google ID token");
                    return BadRequest(new { message = "Invalid Google ID token" });
                }

                var dto = new GoogleUserInfoDto
                {
                    Email = payload.Email,
                    NameIdentifier = payload.Subject,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    Picture = payload.Picture,
                    Locale = payload.Locale
                };

                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    user = _mapper.Map<User>(dto);
                    await _userRepository.CreateAsync(user);
                }
                else if (string.IsNullOrEmpty(user.ExternalProvider))
                {
                    user.ExternalProvider = "Google";
                    user.ExternalId = dto.NameIdentifier;
                    await _userRepository.UpdateAsync(user);
                }

                var jwt = _tokenGenerator.GenerateToken(user);
                var refreshToken = new RefreshToken
                {
                    Token = _tokenService.GenerateRefreshToken(),
                    UserId = user.Id,
                    Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
                };
                refreshToken.Token = _tokenService.HashToken(refreshToken.Token);
                await _refreshTokenRepo.CreateAsync(refreshToken);

                return Ok(new AuthResponseDto
                {
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Token = jwt,
                    RefreshToken = refreshToken.Token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Google SignIn with token failed: {ex.Message}");
                return BadRequest(new { message = "Google SignIn with token failed" });
            }
        }

    }
}
