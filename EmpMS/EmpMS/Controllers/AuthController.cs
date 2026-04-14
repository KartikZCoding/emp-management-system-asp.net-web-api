using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Azure;
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
        private APIResponse _apiResponse;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _apiResponse = new();
        }

        [HasPermission("User.Create")]
        [HttpPost("create-user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateUser(CreateUserDto createUserDto)
        {
            string createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var result = await _authService.CreateUserAsync(createUserDto, createdBy);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;

            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Login(LoginDto loginDto)
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

            _apiResponse.Data = new { response.Username, response.Email, response.Role};
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }


        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            await _authService.SendOtpAsync(forgotPasswordDto);

            _apiResponse.Data = "Otp sent successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [AllowAnonymous]
        [HttpPost("reset-password-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordAsync(resetPasswordDto);

            _apiResponse.Data = "Password changed successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> RefreshToken()
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

            _apiResponse.Data = new { response.Username, response.Email, response.Role};
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

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

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = "Successfull";
            return Ok(_apiResponse);

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

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = "Successfull";
            return Ok(_apiResponse);

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

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = "Successfull";
            return Ok(_apiResponse);

        }
        */

    }
}
