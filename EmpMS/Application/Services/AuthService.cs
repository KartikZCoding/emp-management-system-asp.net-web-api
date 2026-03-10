using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IRoleRepository roleRepository, IJwtHelper jwtHelper, IMemoryCache cache, IEmailService emailService)
        {
            _authRepository = authRepository;
            _roleRepository = roleRepository;
            _jwtHelper = jwtHelper;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await _authRepository.UserExistAsync(dto.Username))
                throw new BadRequestException("User already exists!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _authRepository.CreateUserAsync(user);

            // 5. Assign Default Role "Employee"
            var role = await _roleRepository.GetRoleByNameAsync("Employee");
            if (role == null) throw new NotFoundException("Default role 'Employee' not found.");
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            await _authRepository.AddUserRoleAsync(userRole);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                throw new BadRequestException("User not found!");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!verifyPassword)
                throw new UnauthorizedException("Invalid password!");

            // NEW — fetch ALL privileges for the user's role(s)
            var permissions = await _authRepository.GetUserPermissionsAsync(user.Id);

            // 4. Generate new tokens (token rotation)
            string accessToken = _jwtHelper.GenerateToken(user.Id, user.Username, user.Email, permissions);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            //save refresh token to DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _authRepository.UpdateUserAsync(user);

            var loginResponse = new LoginResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken,
            };

            return loginResponse;
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // 1. Extract claims from the expired access token
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(dto.AccessToken);
            if(principal == null)
                throw new UnauthorizedException("Invalid access token!");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedException("Invalid token claims");

            int userId = int.Parse(userIdClaim);

            // 2. Get user and validate refresh token
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token");

            // NEW — fetch ALL privileges for the user's role(s)
            var permissions = await _authRepository.GetUserPermissionsAsync(userId);

            // 4. Generate new tokens (token rotation)
            string newAccessToken = _jwtHelper.GenerateToken(user.Id, user.Username, user.Email, permissions);
            string newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // 5. Save new refresh token (invalidates old one)
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _authRepository.UpdateUserAsync(user);

            return new LoginResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        public async Task SendOtpAsync(ForgotPasswordDto forgotPasswordDto)
        {
            if (string.IsNullOrEmpty(forgotPasswordDto.Email))
                throw new BadRequestException("Enter a valid email!");

            var user = await _authRepository.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                throw new NotFoundException("User not found!");

            var otp = new Random().Next(100000, 999999).ToString();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _cache.Set(forgotPasswordDto.Email, otp, options);

            string sub = "Reset Password OTP";
            string body = $"Your OTP is: {otp}. Valid upto 5 min!";

            await _emailService.SendEmailAsync(forgotPasswordDto.Email, sub, body);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrEmpty(resetPasswordDto.Otp)) throw new BadRequestException("Enter a OTP!");
            if (string.IsNullOrEmpty(resetPasswordDto.NewPassword)) throw new BadRequestException("Enter a new password!");

            if (!_cache.TryGetValue(resetPasswordDto.Email, out var cachedOtp))
                throw new BadRequestException("OTP expired or invalid email!");

            if (cachedOtp?.ToString() != resetPasswordDto.Otp)
                throw new BadRequestException("Invalid OTP");

            var user = await _authRepository.GetUserByEmailAsync(resetPasswordDto.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);

            await _authRepository.UpdateUserAsync(user);
            _cache.Remove(resetPasswordDto.Email);
        }

/* Don't used */
/*------------------------------------------------------------------------------------------------------*/
/*
        public async Task UserResetPasswordAsync(int userId, ResetPassUserDto dto)
        {
            if (string.IsNullOrEmpty(dto.NewPassword)) throw new BadRequestException("please enter a new password!");

            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found!");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _authRepository.UpdateUserAsync(user);
        }

        public async Task AdminResetPasswordAsync(ResetPassAdminDto dto)
        {
            if (string.IsNullOrEmpty(dto.NewPassword)) throw new BadRequestException("please enter a new password!");

            var user = await _authRepository.GetUserByIdAsync(dto.UserId);
            if (user == null)
                throw new NotFoundException("User not found!");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _authRepository.UpdateUserAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found!");

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
                throw new BadRequestException("Old password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _authRepository.UpdateUserAsync(user);

        }
*/
/*------------------------------------------------------------------------------------------------------*/
    }
}
