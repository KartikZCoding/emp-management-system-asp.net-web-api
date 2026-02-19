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
    public class RolePrivilegesController : ControllerBase
    {
        private readonly IRolePrivilegeService _rolePrivilegeService; 
        private APIResponse _apiResponse;

        public RolePrivilegesController(IRolePrivilegeService rolePrivilegeService)
        {
            _rolePrivilegeService = rolePrivilegeService;
            _apiResponse = new();
        }

        [HttpPost("assign-privilege")]
        public async Task<ActionResult<APIResponse>> AssignPrivilege(RolePrivilegeDto rolePrivilegeDto)
        {
            try
            {
                await _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto);

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

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<APIResponse>> GetPrivilegeByRole(int roleId)
        {
            try
            {
                var privileges = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(roleId);

                _apiResponse.Data = privileges;
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

        [HttpDelete("{roleId}/{privilegeId}")]
        public async Task<ActionResult<APIResponse>> RemovePrivilege(int roleId, int privilegeId)
        {
            try
            {
                await _rolePrivilegeService.RemovePrivilegeFromRoleAsync(roleId, privilegeId);

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
