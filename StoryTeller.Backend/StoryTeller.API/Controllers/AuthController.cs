using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;

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
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            _logger.LogInfo($"Login DTO received: Email = {dto.Email}, Password = {dto.Password}");
            var result = await _authService.LoginAsync(dto);
            _logger.LogInfo($"User {dto.Email} logged in.");
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            _logger.LogInfo($"Token refreshed for {dto.Email}.");
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto);
            _logger.LogInfo($"User {dto.Email} logged out.");
            return NoContent();
        }
    }
}
