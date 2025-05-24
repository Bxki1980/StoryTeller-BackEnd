using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Google.Apis.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Enums;
using Microsoft.AspNetCore.Authentication.Google;


namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers
{



    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly TokenService _tokenService;
        private readonly JwtTokenGenerator _tokenGenerator;



        public AuthController(IAuthServices authService, ILoggerManager logger, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepo, TokenService tokenService, JwtTokenGenerator tokenGenerator)
        {
            _logger = logger;
            _userRepository = userRepository;
            _refreshTokenRepo = refreshTokenRepo;
            _tokenService = tokenService;
            _tokenGenerator = tokenGenerator;
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignupDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                _logger.LogInfo($"User {dto.Email} signed up.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Signup failed: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> login(UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                _logger.LogError($"User {dto.Email} Failed to login");
                return Unauthorized();
            }
            else 
            { 
                _logger.LogInfo($"User {dto.Email} logged in.");
            return Ok(result);

            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto dto)
        {

            var existing = await _refreshTokenRepo.GetByTokenAsync(dto.RefreshToken);
            if (existing == null || existing.Expires < DateTime.UtcNow || existing.Revoked)
                return Unauthorized("Invalid refresh token");

            // ✅ Revoke old token
            await _refreshTokenRepo.InvalidateAsync(existing.Token);

            // ✅ Get user
            var user = await _userRepository.GetByIdAsync(existing.UserId);
            if (user == null)
                return Unauthorized("User not found");

            // ✅ Issue new refresh token
            var newToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            await _refreshTokenRepo.CreateAsync(newToken);

            return Ok(new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),
                RefreshToken = newToken.Token
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto);
            return NoContent();
        }


    }

    [ApiController]
    [Route("api/[controller]")]
    public class GoogleLoginController : ControllerBase
    {


        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly TokenService _tokenService;
        private readonly JwtTokenGenerator _tokenGenerator;


        public GoogleLoginController(IUserRepository userRepo, ILoggerManager logger)
        {
            _userRepository = userRepo;
            _logger = logger;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleLogin))
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {




            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Unauthorized("Google Login failed");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var nameId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nameId))
            {
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
            }
            else if (string.IsNullOrEmpty(user.ExternalProvider))
            {
                user.ExternalProvider = "Google";
                user.ExternalId = nameId;
                await _userRepository.UpdateAsync(user);
            }

            // Issure JWT
            var jwt = _tokenGenerator.GenerateToken(user);

            // Create refresh token
            var refreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            refreshToken.Token = _tokenService.HashToken(refreshToken.Token);
            await _refreshTokenRepo.CreateAsync(refreshToken);

            return Ok(new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = jwt,
                RefreshToken = refreshToken.Token // return hashed or raw token based on your design
            });
        }

        [HttpPost("google-signin-token")]
        public async Task<IActionResult> GoogleSignInWithToken([FromBody] string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            var email = payload.Email;
            var nameId = payload.Subject;

            // TODO: check email domain, fetch/create user, issue your own JWT

            return Ok(new
            {
                Email = payload.Email,
                Name = payload.Name,
                Picture = payload.Picture
            });
        }
    }
}
