using EmpMS.DTOs.Auth;
using EmpMS.Models;
using EmpMS.Repositories;

namespace EmpMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await _authRepository.UserExistAsync(dto.Username))
                throw new Exception("User already exists!");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _authRepository.CreateUserAsync(user);
        }
    }
}
