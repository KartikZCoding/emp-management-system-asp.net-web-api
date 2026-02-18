using EmpMS.DTOs.Auth;
using EmpMS.Helpers;
using EmpMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost]
        public async Task<ActionResult<APIResponse>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                await _authService.RegisterAsync(registerDto);

                _apiResponse.Data = "Successfull";
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }
    }
}
