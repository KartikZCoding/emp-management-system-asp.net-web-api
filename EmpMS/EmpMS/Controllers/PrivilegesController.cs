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
    public class PrivilegesController : ControllerBase
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<PrivilegeResponseDto>>>> GetAllPrivileges()
        {
            var response = await _privilegeService.GetAllPrivilegesAsync();
            return Ok(new APIResponse<List<PrivilegeResponseDto>>(response));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PrivilegeResponseDto>>> GetPrivilegeById(int id)
        {
            var response = await _privilegeService.GetPrivilegeByIdAsync(id);
            return Ok(new APIResponse<PrivilegeResponseDto>(response));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreatePrivilege(PrivilegeDto privilegeDto)
        {
            await _privilegeService.CreatePrivilegeAsync(privilegeDto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Privilege created successfully"
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdatePrivilege(int id, PrivilegeDto privilegeDto)
        {
            await _privilegeService.UpdatePrivilegeAsync(id, privilegeDto);

            return Ok(new APIResponse { Message = "Privilege updated successfully" });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeletePrivilege(int id)
        {
            await _privilegeService.DeletePrivilegeAsync(id);

            return Ok(new APIResponse { Message = "Privilege deleted successfully" });
        }
    }
}
