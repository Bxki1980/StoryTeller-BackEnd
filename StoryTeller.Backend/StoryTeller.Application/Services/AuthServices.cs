using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StoryTeller.Shared.Exceptions;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Exceptions;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using System.Security.Authentication;

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
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;



        public AuthServices(
            IMapper mapper, 
            IConfiguration config, 
            ILoggerManager logger, 
            IUserRepository userRepository, 
            JwtTokenGenerator tokenGenerator, 
            TokenService tokenService, 
            IRefreshTokenRepository refreshTokenRepo, 
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> options)
        {
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _tokenGenerator = tokenGenerator;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _unitOfWork = unitOfWork;
            _jwtSettings = options.Value;
        }


        public async Task<AuthResponseDto> RegisterAsync(UserSignupDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ConflictException("User already exists");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var refreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            };

            var success = await _unitOfWork.CommitUserWithTokenAsync(user, refreshToken);
            if (!success)
                throw new Exception("Failed to register user");

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenGenerator.GenerateToken(user),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    throw new UserNotFoundException(dto.Email);
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    throw new InvalidCredentialsException();
                }

                // Generate refresh token
                var refreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = _tokenService.GenerateRefreshToken(),
                    Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
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
            if (user == null) throw new UserAlreadyExistsException(dto.Email);

            var tokenHash = _tokenService.HashToken(dto.RefreshToken);
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(tokenHash);

            if ( storedToken == null || storedToken.UserId != user.Id || storedToken.Expires < DateTime.UtcNow || storedToken.Revoked)
            {
                throw new InvalidRefreshTokenException();
            }

            // Revoke old token
            await _refreshTokenRepo.InvalidateAsync(storedToken.Token);



            // Generate new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
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
