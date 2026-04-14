using Application.Common;
using Application.DTOs.Leave;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
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

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("types")]
        [HasPermission("Leave.Read")]
        public async Task<ActionResult<APIResponse<List<LeaveTypeResponseDto>>>> GetAllLeaveTypes()
        {
            var response = await _leaveService.GetAllLeaveTypesAsync();
            return Ok(new APIResponse<List<LeaveTypeResponseDto>>(response));
        }

        [HttpGet("types/{id}")]
        [HasPermission("Leave.Read")]
        public async Task<ActionResult<APIResponse<LeaveTypeResponseDto>>> GetLeaveTypeById(int id)
        {
            var response = await _leaveService.GetLeaveTypeByIdAsync(id);
            return Ok(new APIResponse<LeaveTypeResponseDto>(response));
        }

        [HttpPost("types")]
        [HasPermission("Leave.Create")]
        public async Task<ActionResult<APIResponse<LeaveTypeResponseDto>>> CreateLeaveType(LeaveTypeDto dto)
        {
            var response = await _leaveService.CreateLeaveTypeAsync(dto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<LeaveTypeResponseDto>(response)
            {
                StatusCode = HttpStatusCode.Created
            });
        }

        [HttpPut("types/{id}")]
        [HasPermission("Leave.Update")]
        public async Task<ActionResult<APIResponse<LeaveTypeResponseDto>>> UpdateLeaveType(int id, LeaveTypeDto dto)
        {
            var response = await _leaveService.UpdateLeaveTypeAsync(id, dto);
            return Ok(new APIResponse<LeaveTypeResponseDto>(response));
        }

        [HttpDelete("types/{id}")]
        [HasPermission("Leave.Delete")]
        public async Task<ActionResult<APIResponse>> DeleteLeaveType(int id)
        {
            await _leaveService.DeleteLeaveTypeAsync(id);

            return Ok(new APIResponse { Message = "Leave type deleted successfully" });
        }

        [HttpGet("balances")]
        [Authorize]
        public async Task<ActionResult<APIResponse<List<LeaveBalanceResponseDto>>>> GetMyBalances([FromQuery] int year)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.GetMyBalancesAsync(email, year);
            return Ok(new APIResponse<List<LeaveBalanceResponseDto>>(response));
        }

        [HttpPost("balances/assign")]
        [HasPermission("Leave.Create")]
        public async Task<ActionResult<APIResponse>> AssignBalances([FromQuery] int employeeId, [FromQuery] int year)
        {
            await _leaveService.AssignBalancesAsync(employeeId, year);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Leave balances assigned successfully"
            });
        }

        [HttpPost("requests")]
        [Authorize]
        public async Task<ActionResult<APIResponse<LeaveRequestResponseDto>>> ApplyLeave(LeaveRequestDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.ApplyLeaveAsync(email, dto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<LeaveRequestResponseDto>(response)
            {
                StatusCode = HttpStatusCode.Created
            });
        }

        [HttpGet("requests/my")]
        [Authorize]
        public async Task<ActionResult<APIResponse<List<LeaveRequestResponseDto>>>> GetMyRequests()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.GetMyRequestsAsync(email);
            return Ok(new APIResponse<List<LeaveRequestResponseDto>>(response));
        }

        [HttpGet("requests/pending")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse<List<LeaveRequestResponseDto>>>> GetPendingRequests()
        {
            var response = await _leaveService.GetPendingRequestAsync();
            return Ok(new APIResponse<List<LeaveRequestResponseDto>>(response));
        }

        [HttpPut("requests/{id}/approve")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse<LeaveRequestResponseDto>>> ApproveLeave(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _leaveService.ApproveLeaveAsync(id, userId, decisionNote);
            return Ok(new APIResponse<LeaveRequestResponseDto>(response));
        }

        [HttpPut("requests/{id}/reject")]
        [HasPermission("LeaveRequest.Update")]
        public async Task<ActionResult<APIResponse<LeaveRequestResponseDto>>> RejectLeave(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _leaveService.RejectLeaveAsync(id, userId, decisionNote);
            return Ok(new APIResponse<LeaveRequestResponseDto>(response));
        }

        [HttpPut("requests/{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<APIResponse<LeaveRequestResponseDto>>> CancelLeave(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _leaveService.CancelLeaveAsync(email, id);
            return Ok(new APIResponse<LeaveRequestResponseDto>(response));
        }
    }
}
