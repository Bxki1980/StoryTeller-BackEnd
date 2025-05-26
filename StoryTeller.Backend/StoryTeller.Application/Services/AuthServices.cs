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
        private readonly TokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly int _refreshTokenExpiryDays;



        public AuthServices(IMapper mapper, IConfiguration config, ILoggerManager logger, IUserRepository userRepository, JwtTokenGenerator tokenGenerator, TokenService tokenService, IRefreshTokenRepository refreshTokenRepo)
        {
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _tokenGenerator = tokenGenerator;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _refreshTokenExpiryDays = int.TryParse(_config["Jwt:RefreshTokenExpiryDays"], out var days) ? days : 7;
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

            await _userRepository.CreateAsync(user);

            _logger.LogInfo($"User registered {user.Email}");

            var refreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
            };
            await _refreshTokenRepo.CreateAsync(refreshToken);

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),
                RefreshToken = refreshToken.Token,

            };
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    throw new Exception("Invalid credentials");
                }

                // Generate refresh token
                var refreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = _tokenService.GenerateRefreshToken(),
                    Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
                };
                await _refreshTokenRepo.CreateAsync(refreshToken);

                _logger.LogInfo($"User logged in: {user.Email}");

                return new AuthResponseDto
                {
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Token = _tokenGenerator.GenerateToken(user),
                    RefreshToken = refreshToken.Token,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login failed for {dto.Email}: {ex.Message}");
                throw; 
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto) 
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) throw new Exception("User not found");

            var tokenHash = _tokenService.HashToken(dto.RefreshToken);
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(tokenHash);

            if ( storedToken == null || storedToken.UserId != user.Id || storedToken.Expires < DateTime.UtcNow || storedToken.Revoked)
            {
                throw new Exception("Invalid or expired refresh token");
            }

            // Revoke old token
            await _refreshTokenRepo.InvalidateAsync(storedToken.Token);



            // Generate new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            };

            newRefreshToken.Token = _tokenService.HashToken(newRefreshToken.Token); // hash before saving
            await _refreshTokenRepo.CreateAsync(newRefreshToken);

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),
                RefreshToken = newRefreshToken.Token, // hashed version returned — or return original before hashing
            };
        }


        public async Task LogoutAsync(RefreshRequestDto dto)
        {
            var tokenHash = _tokenService.HashToken(dto.RefreshToken);
            await _refreshTokenRepo.InvalidateAsync(tokenHash);
        }


    }
}
