using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Exceptions;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HasPermission("User.Create")]
        [HttpPost("create-user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<CreateUserResponseDto>>> CreateUser(CreateUserDto createUserDto)
        {
            string createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var result = await _authService.CreateUserAsync(createUserDto, createdBy);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<CreateUserResponseDto>(result)
            {
                StatusCode = HttpStatusCode.Created
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            Response.Cookies.Append("accessToken", response.Token, new CookieOptions
            {
                HttpOnly = true,  // JavaScript CANNOT read this cookie (prevents XSS)
                Secure = true,        // Cookie only sent over HTTPS
                SameSite = SameSiteMode.Strict, // Cookie not sent on cross-site requests (prevents CSRF)
                Expires = DateTime.UtcNow.AddMinutes(15)  // Match token expiry
            });

            Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new APIResponse<object>(new { response.Username, response.Email, response.Role }));
        }


        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            await _authService.SendOtpAsync(forgotPasswordDto);

            return Ok(new APIResponse { Message = "OTP sent successfully" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordAsync(resetPasswordDto);

            return Ok(new APIResponse { Message = "Password changed successfully" });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> RefreshToken()
        {
            var accessToken = Request.Cookies["accessToken"];
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedException("Missing tokens");

            var dto = new RefreshTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var response = await _authService.RefreshTokenAsync(dto);

            Response.Cookies.Append("accessToken", response.Token, new CookieOptions
            {
                HttpOnly = true,  // JavaScript CANNOT read this cookie (prevents XSS)
                Secure = true,        // Cookie only sent over HTTPS
                SameSite = SameSiteMode.Strict, // Cookie not sent on cross-site requests (prevents CSRF)
                Expires = DateTime.UtcNow.AddMinutes(15)  // Match token expiry
            });

            Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new APIResponse<object>(new { response.Username, response.Email, response.Role }));
        }


        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _authService.ChangePasswordAsync(userId, changePasswordDto);

            return Ok(new APIResponse { Message = "Password changed successfully" });
        }

        /*[Authorize]
        [HttpPost("user-reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UserResetPassword(ResetPassUserDto resetPassUserDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _authService.UserResetPasswordAsync(userId, resetPassUserDto);

            return Ok(new APIResponse { Message = "Password reset successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin-reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AdminResetPassword(ResetPassAdminDto resetPassAdminDto)
        {

            await _authService.AdminResetPasswordAsync(resetPassAdminDto);

            return Ok(new APIResponse { Message = "Password reset successfully" });
        }
        */

    }
}
