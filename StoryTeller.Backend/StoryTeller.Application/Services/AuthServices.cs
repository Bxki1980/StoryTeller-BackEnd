using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtTokenGenerator _tokenGenerator;


        public AuthServices(IMapper mapper, IConfiguration config, ILoggerManager logger, IUserRepository userRepository, JwtTokenGenerator tokenGenerator)
        {
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _tokenGenerator = tokenGenerator;
        }


        public async Task<AuthResponseDto> RegisterAsync(UserSignupDto dto)
        {
            // Check if the user already exists
            var exisitingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if ( exisitingUser != null)
            {
                throw new Exception("User already exist. ");
            }

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.CreateASync(user);

            _logger.LogInfo($"User registered {user.Email}");

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),

            };
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid credentials");
            }

            _logger.LogInfo($"User logged in: {user.Email}");

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),
            };
        }
    }
}
