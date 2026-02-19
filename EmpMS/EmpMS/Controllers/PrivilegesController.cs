using EmpMS.DTOs.Auth;
using EmpMS.Helpers;
using EmpMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PrivilegesController : ControllerBase
    {
        private readonly IPrivilegeService _privilegeService;
        private APIResponse _apiResponse;

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
            _apiResponse = new();
        }

        [HttpGet("all")]
        public async Task<ActionResult<APIResponse>> GetAllPrivileges()
        {
            try
            {
                var response = await _privilegeService.GetAllPrivilegesAsync();

                _apiResponse.Data = response;
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

        [HttpGet("id/{id}")]
        public async Task<ActionResult<APIResponse>> GetPrivilegeById(int id)
        {
            try
            {
                var response = await _privilegeService.GetPrivilegeByIdAsync(id);

                _apiResponse.Data = response;
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

        [HttpPost("create")]
        public async Task<ActionResult<APIResponse>> CreatePrivilege(PrivilegeDto privilegeDto)
        {
            try
            {
                await _privilegeService.CreatePrivilegeAsync(privilegeDto);

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

        [HttpPut("update/{id}")]
        public async Task<ActionResult<APIResponse>> UpdatePrivilege(int id, PrivilegeDto privilegeDto)
        {
            try
            {
                await _privilegeService.UpdatePrivilegeAsync(id, privilegeDto);

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

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<APIResponse>> DeletePrivilege(int id)
        {
            try
            {
                await _privilegeService.DeletePrivilegeAsync(id);

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
