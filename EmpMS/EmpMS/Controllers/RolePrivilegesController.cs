using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission("Privilege.Manage")]
    public class RolePrivilegesController : ControllerBase
    {
        private readonly IRolePrivilegeService _rolePrivilegeService;

        public RolePrivilegesController(IRolePrivilegeService rolePrivilegeService)
        {
            _rolePrivilegeService = rolePrivilegeService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AssignPrivilege(RolePrivilegeDto rolePrivilegeDto)
        {
            await _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Privilege assigned successfully"
            });
        }

        [HttpGet("role/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<PrivilegeResponseDto>>>> GetPrivilegeByRole(int roleId)
        {
            var privileges = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(roleId);
            return Ok(new APIResponse<List<PrivilegeResponseDto>>(privileges));
        }

        [HttpDelete("{roleId}/{privilegeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> RemovePrivilege(int roleId, int privilegeId)
        {
            await _rolePrivilegeService.RemovePrivilegeFromRoleAsync(roleId, privilegeId);

            return Ok(new APIResponse { Message = "Privilege removed successfully" });
        }
    }
}
