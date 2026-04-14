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
    [HasPermission("Role.Manage")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private APIResponse _apiResponse;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
            _apiResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllRoles()
        {

            var response = await _roleService.GetAllRolesAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateRole(RoleDto roleDto)
        {

            await _roleService.CreateRoleAsync(roleDto);

            _apiResponse.Data = "Successfull";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return StatusCode(StatusCodes.Status201Created, _apiResponse);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateRole(int id, RoleDto roleDto)
        {

            await _roleService.UpdateRoleAsync(id, roleDto);

            _apiResponse.Data = "Successfull";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteRole(int id)
        {

            await _roleService.DeleteRoleAsync(id);

            _apiResponse.Data = "Successfull";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }
    }
}
