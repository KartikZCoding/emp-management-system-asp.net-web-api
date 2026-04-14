using Application.Common;
using Application.DTOs.Attendance;
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
    public class AttendanceRegularizationController : ControllerBase
    {
        private readonly IAttendanceRegularizationService _attendanceRegularizationService;

        public AttendanceRegularizationController(IAttendanceRegularizationService attendanceRegularizationService)
        {
            _attendanceRegularizationService = attendanceRegularizationService;
        }

        [HttpPost]
        [Route("request")]
        [Authorize]
        public async Task<ActionResult<APIResponse<AttendanceRegularizationResponseDto>>> CreateRequestAsync(AttendanceRegularizationRequestDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if(string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.CreateRequestAsync(email, dto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<AttendanceRegularizationResponseDto>(response)
            {
                StatusCode = HttpStatusCode.Created
            });
        }

        [HttpGet]
        [Route("my-request")]
        [Authorize]
        public async Task<ActionResult<APIResponse<List<AttendanceRegularizationResponseDto>>>> GetMyRequestAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.GetMyRequestsAsync(email);
            return Ok(new APIResponse<List<AttendanceRegularizationResponseDto>>(response));
        }

        [HttpGet]
        [Route("pending")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse<List<AttendanceRegularizationResponseDto>>>> GetPending()
        {
            var response = await _attendanceRegularizationService.GetPendingRequestsAsync();
            return Ok(new APIResponse<List<AttendanceRegularizationResponseDto>>(response));
        }

        [HttpPut("{id}/approve")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse<AttendanceRegularizationResponseDto>>> Approve(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _attendanceRegularizationService.ApproveAsync(id, userId, decisionNote);
            return Ok(new APIResponse<AttendanceRegularizationResponseDto>(response));
        }

        [HttpPut("{id}/reject")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse<AttendanceRegularizationResponseDto>>> Reject(int id, [FromQuery] string? decisionNote)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _attendanceRegularizationService.RejectAsync(id, userId, decisionNote);
            return Ok(new APIResponse<AttendanceRegularizationResponseDto>(response));
        }

        // GET: api/AttendanceRegularization/missed-checkouts → Employee sees missed dates
        [HttpGet("missed-checkouts")]
        [Authorize]
        public async Task<ActionResult<APIResponse<List<AttendanceResponseDto>>>> GetMissedCheckouts()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.GetMissedCheckoutsAsync(email);
            return Ok(new APIResponse<List<AttendanceResponseDto>>(response));
        }
    }
}
