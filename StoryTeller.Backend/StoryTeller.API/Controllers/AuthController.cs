using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly ILoggerManager _logger;

        public AuthController(IAuthServices authService, ILoggerManager logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignupDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            _logger.LogInfo($"User {dto.Email} signed up.");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            _logger.LogInfo($"User {dto.Email} logged in.");
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            _logger.LogInfo($"Token refreshed for {dto.Email}.");
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto);
            _logger.LogInfo($"User {dto.Email} logged out.");
            return NoContent();
        }
    }
}
