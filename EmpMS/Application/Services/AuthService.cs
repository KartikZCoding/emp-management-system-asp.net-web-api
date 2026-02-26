using Application.Interfaces;
using Application.DTOs.Auth;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtHelper _jwtHelper;

        public AuthService(IAuthRepository authRepository, IRoleRepository roleRepository, IJwtHelper jwtHelper)
        {
            _authRepository = authRepository;
            _roleRepository = roleRepository;
            _jwtHelper = jwtHelper;
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

            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole == null)
                throw new UnauthorizedException("User has no role assigned!");
            var role = userRole.Role;

            string token = _jwtHelper.GenerateToken(user.Id, user.Username, role.RoleName);

            var loginResponse = new LoginResponseDto
            {
                Username = user.Username,
                Token = token,
                Role = role.RoleName
            };

            return loginResponse;
        }

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

    }
}
