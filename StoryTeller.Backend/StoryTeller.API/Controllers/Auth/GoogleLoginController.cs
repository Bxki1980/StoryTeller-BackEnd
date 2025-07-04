﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Common;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.API.Controllers.auth
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
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IGoogleClaimsParser _claimsParser;

        public GoogleLoginController(
            ILoggerManager logger,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepo, 
            IGoogleAuthService googleAuthService,
            TokenService tokenService,
            JwtTokenGenerator tokenGenerator,
            IMapper mapper,
            IConfiguration config,
            IGoogleClaimsParser claimsParser)
        {
            _logger = logger;
            _userRepository = userRepository;
            _refreshTokenRepo = refreshTokenRepo;
            _tokenService = tokenService;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _googleAuthService = googleAuthService;
            _claimsParser = claimsParser;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return Unauthorized("Google login failed");

            var response = await _googleAuthService.HandleGoogleCallbackAsync(result.Principal);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response));
        }


        [HttpPost("google-signin-token")]
        public async Task<IActionResult> GoogleSignInWithToken([FromBody] string idToken)
        {
            var response = await _googleAuthService.HandleGoogleSignInTokenAsync(idToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response));
        }
    }
}
