using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<CreateUserResponseDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task SendOtpAsync(ForgotPasswordDto forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);

        //Task UserResetPasswordAsync(int userId, ResetPassUserDto dto);
        //Task AdminResetPasswordAsync(ResetPassAdminDto dto);
    }
}
