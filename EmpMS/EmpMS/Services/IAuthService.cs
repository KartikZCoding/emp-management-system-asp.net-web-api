using EmpMS.DTOs.Auth;

namespace EmpMS.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task UserResetPasswordAsync(int userId, ResetPassUserDto dto);
        Task AdminResetPasswordAsync(ResetPassAdminDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
