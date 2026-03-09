using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task SendOtpAsync(ForgotPasswordDto forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        //Task UserResetPasswordAsync(int userId, ResetPassUserDto dto);
        //Task AdminResetPasswordAsync(ResetPassAdminDto dto);
        //Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
