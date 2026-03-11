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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IRoleRepository roleRepository, IEmployeeRepository employeeRepository, IJwtHelper jwtHelper, IMemoryCache cache, IEmailService emailService)
        {
            _authRepository = authRepository;
            _roleRepository = roleRepository;
            _employeeRepository = employeeRepository;
            _jwtHelper = jwtHelper;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<CreateUserResponseDto> CreateUserAsync(CreateUserDto dto, string createdBy)
        {
            // 1. Validate username uniqueness
            if (await _authRepository.UserExistAsync(dto.Username))
                throw new BadRequestException("Username already exists!");
            // 2. Validate email uniqueness
            if (await _authRepository.EmailExistsAsync(dto.Email))
                throw new BadRequestException("Email already exists!");
            // 3. If EmployeeId given, validate it
            if (dto.EmployeeId.HasValue)
            {
                var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId.Value);
                if (employee == null)
                    throw new NotFoundException("Employee not found!");
                if (await _authRepository.EmployeeHasUserAsync(dto.EmployeeId.Value))
                    throw new BadRequestException("This employee already has a user account!");
            }

            // 4. Generate temporary password
            string tempPassword = GenerateTemporaryPassword();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            // 5. Create user
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.Now,
                EmployeeId = dto.EmployeeId,
                MustChangePassword = true,
                CreatedBy = createdBy
            };

            // 6. Assign roles
            var roleNames = new List<string>();
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleRepository.GetRoleByIdAsync(roleId);
                if (role == null) throw new NotFoundException($"Role with ID {roleId} not found!");
                await _authRepository.CreateUserAsync(user);
                await _authRepository.AddUserRoleAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                });
                roleNames.Add(role.RoleName);
            }

            // 7. Send welcome email with temp password
            string subject = "Your EmpMS Account Has Been Created";
            string body = $"Hello,\n\n"
                + $"Your account has been created in the Employee Management System.\n\n"
                + $"Username: {dto.Username}\n"
                + $"Temporary Password: {tempPassword}\n\n"
                + $"Please login and change your password immediately.\n\n"
                + $"Regards,\nAdmin Team";
            await _emailService.SendEmailAsync(dto.Email, subject, body);
            // 8. Return response
            return new CreateUserResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roleNames,
                EmployeeId = user.EmployeeId
            };
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
                MustChangePassword = user.MustChangePassword
            };

            return loginResponse;
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // 1. Extract claims from the expired access token
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found!");

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
                throw new BadRequestException("Old password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.MustChangePassword = false;

            await _authRepository.UpdateUserAsync(user);

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
        */
        /*------------------------------------------------------------------------------------------------------*/

        private string GenerateTemporaryPassword(int length = 12)
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnpqrstuvwxyz";
            const string digits = "23456789";
            const string special = "!@#$%";
            var random = new Random();
            var password = new List<char>
            {
                upper[random.Next(upper.Length)],
                lower[random.Next(lower.Length)],
                digits[random.Next(digits.Length)],
                special[random.Next(special.Length)]
            };
            string allChars = upper + lower + digits + special;
            for (int i = password.Count; i < length; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }
            // Shuffle
            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }
    }
}
