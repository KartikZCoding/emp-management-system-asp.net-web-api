using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
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

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> RegisterAsync(RegisterDto registerDto)
        {
            await _authService.RegisterAsync(registerDto);

            _apiResponse.Data = "Successfull";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [Authorize]
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

        [Authorize]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var reponse = await _authService.RefreshTokenAsync(refreshTokenDto);

            _apiResponse.Data = reponse;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }

    }
}
