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
        private APIResponse _apiResponse;

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
            _apiResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllPrivileges()
        {

            var response = await _privilegeService.GetAllPrivilegesAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetPrivilegeById(int id)
        {

            var response = await _privilegeService.GetPrivilegeByIdAsync(id);

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
        public async Task<ActionResult<APIResponse>> CreatePrivilege(PrivilegeDto privilegeDto)
        {

            await _privilegeService.CreatePrivilegeAsync(privilegeDto);

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
        public async Task<ActionResult<APIResponse>> UpdatePrivilege(int id, PrivilegeDto privilegeDto)
        {

            await _privilegeService.UpdatePrivilegeAsync(id, privilegeDto);

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
        public async Task<ActionResult<APIResponse>> DeletePrivilege(int id)
        {

            await _privilegeService.DeletePrivilegeAsync(id);

            _apiResponse.Data = "Successfull";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);

        }
    }
}
