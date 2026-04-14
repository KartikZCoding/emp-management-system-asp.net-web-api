using Application.Common;
using Application.DTOs.Designation;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesignationsController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationsController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        [HttpGet]
        [HasPermission("Designation.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<DesignationResponseDto>>>> GetAllDesignations()
        {
            var response = await _designationService.GetAllDesignationsAsync();
            return Ok(new APIResponse<List<DesignationResponseDto>>(response));
        }

        [HttpGet("{id}")]
        [HasPermission("Designation.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<DesignationResponseDto>>> GetDesignationById(int id)
        {
            var response = await _designationService.GetDesignationByIdAsync(id);
            return Ok(new APIResponse<DesignationResponseDto>(response));
        }

        [HttpPost]
        [HasPermission("Designation.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateDesignation(DesignationDto designationDto)
        {
            await _designationService.CreateDesignationAsync(designationDto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Designation created successfully"
            });
        }

        [HttpPut("{id}")]
        [HasPermission("Designation.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateDesignation(int id, DesignationDto designationDto)
        {
            await _designationService.UpdateDesignationAsync(id, designationDto);

            return Ok(new APIResponse { Message = "Designation updated successfully" });
        }

        [HttpDelete("{id}")]
        [HasPermission("Designation.Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteDesignation(int id)
        {
            await _designationService.DeleteDesignationAsync(id);

            return Ok(new APIResponse { Message = "Designation deleted successfully" });
        }
    }
}
