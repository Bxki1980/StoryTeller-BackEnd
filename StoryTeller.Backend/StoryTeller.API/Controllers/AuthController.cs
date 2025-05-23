using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;

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
        private readonly AuthServices _authServices;



        public AuthController(IAuthServices authService, ILoggerManager logger, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepo, TokenService tokenService, JwtTokenGenerator tokenGenerator, AuthServices authServices)
        {
            _authService = authService;
            _logger = logger;
            _userRepository = userRepository;
            _refreshTokenRepo = refreshTokenRepo;
            _tokenService = tokenService;
            _tokenGenerator = tokenGenerator;
            _authServices = authServices;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignupDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            _logger.LogInfo($"User {dto.Email} signed up.");
            return Ok(result);
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
}
