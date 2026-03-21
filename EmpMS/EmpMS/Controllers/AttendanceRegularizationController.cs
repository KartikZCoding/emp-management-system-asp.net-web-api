using Application.Common;
using Application.DTOs.Attendance;
using Application.Interfaces;
using Domain.Interfaces;
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
    public class AttendanceRegularizationController : ControllerBase
    {
        private readonly IAttendanceRegularizationService _attendanceRegularizationService;
        private APIResponse _apiResponse;

        public AttendanceRegularizationController(IAttendanceRegularizationService attendanceRegularizationService)
        {
            _attendanceRegularizationService = attendanceRegularizationService;
            _apiResponse = new APIResponse();
        }

        [HttpPost]
        [Route("request")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CreateRequestAsync(AttendanceRegularizationRequestDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if(string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.CreateRequestAsync(email, dto);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;

            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [HttpGet]
        [Route("my-request")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetMyRequestAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.GetMyRequestsAsync(email);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("pending")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse>> GetPending()
        {
            var response = await _attendanceRegularizationService.GetPendingRequestsAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("{id}/approve")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse>> Approve(int id, [FromQuery] string? decisionNote)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _attendanceRegularizationService.ApproveAsync(id, userId, decisionNote);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("{id}/reject")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse>> Reject(int id, [FromQuery] string? decisionNote)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _attendanceRegularizationService.RejectAsync(id, userId, decisionNote);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        // GET: api/AttendanceRegularization/missed-checkouts → Employee sees missed dates
        [HttpGet("missed-checkouts")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetMissedCheckouts()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _attendanceRegularizationService.GetMissedCheckoutsAsync(email);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
    }
}
