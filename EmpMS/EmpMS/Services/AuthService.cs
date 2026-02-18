using BCrypt.Net;
using EmpMS.DTOs.Auth;
using EmpMS.Helpers;
using EmpMS.Models;
using EmpMS.Repositories;

namespace EmpMS.Services
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found!");

            bool isOldPasswordValid  = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
                throw new Exception("Old password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _authRepository.UpdateUserAsync(user);

        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                throw new Exception("User not found!");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!verifyPassword)
                throw new Exception("Invalid password!");

            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole == null)
                throw new Exception("User has no role assigned!");
            var role = userRole.Role;

            string token = _jwtHelper.GenerateToken(user.Id, user.Username, role);

            var loginResponse = new LoginResponseDto
            {
                Username = user.Username,
                Token = token,
                Role = role.RoleName
            };

            return loginResponse;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await _authRepository.UserExistAsync(dto.Username))
                throw new Exception("User already exists!");

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
            if (role == null) throw new Exception("Default role 'Employee' not found.");
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            await _authRepository.AddUserRoleAsync(userRole);
        }
    }
}
