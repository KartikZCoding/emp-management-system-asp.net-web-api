using Application.Common;
using Application.DTOs.Leave;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private APIResponse _apiResponse;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
            _apiResponse = new APIResponse();
        }

        [HttpGet("types")]
        [HasPermission("Leave.Read")]
        public async Task<ActionResult<APIResponse>> GetAllLeaveTypes()
        {
            var response = await _leaveService.GetAllLeaveTypesAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("types/{id}")]
        [HasPermission("Leave.Read")]
        public async Task<ActionResult<APIResponse>> GetLeaveTypeById(int id)
        {
            var response = await _leaveService.GetLeaveTypeByIdAsync(id);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPost("types")]
        [HasPermission("Leave.Create")]
        public async Task<ActionResult<APIResponse>> CreateLeaveType(LeaveTypeDto dto)
        {
            var response = await _leaveService.CreateLeaveTypeAsync(dto);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [HttpPut("types/{id}")]
        [HasPermission("Leave.Update")]
        public async Task<ActionResult<APIResponse>> UpdateLeaveType(int id, LeaveTypeDto dto)
        {
            var response = await _leaveService.UpdateLeaveTypeAsync(id, dto);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpDelete("types/{id}")]
        [HasPermission("Leave.Delete")]
        public async Task<ActionResult<APIResponse>> DeleteLeaveType(int id)
        {
            await _leaveService.DeleteLeaveTypeAsync(id);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("balances")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetMyBalances([FromQuery] int year)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.GetMyBalancesAsync(email, year);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPost("balances/assign")]
        [HasPermission("Leave.Create")]
        public async Task<ActionResult<APIResponse>> AssignBalances([FromQuery] int employeeId, [FromQuery] int year)
        {
            await _leaveService.AssignBalancesAsync(employeeId, year);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [HttpPost("requests")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> ApplyLeave(LeaveRequestDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.ApplyLeaveAsync(email, dto);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [HttpGet("requests/my")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetMyRequests()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.GetMyRequestsAsync(email);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("requests/pending")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse>> GetPendingRequests()
        {
            var response = await _leaveService.GetPendingRequestAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("requests/{id}/approve")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse>> ApproveLeave(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _leaveService.ApproveLeaveAsync(id, userId, decisionNote);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("requests/{id}/reject")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse>> RejectLeave(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _leaveService.RejectLeaveAsync(id, userId, decisionNote);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("requests/{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CancelLeave(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.CancelLeaveAsync(email, id);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
    }
}
