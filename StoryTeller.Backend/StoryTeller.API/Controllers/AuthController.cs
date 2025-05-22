using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
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


    }
}
