using Application.Common;
using Application.DTOs.Attendance;
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
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private APIResponse _apiResponse;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
            _apiResponse = new APIResponse();
        }

        [HttpPost("check-in")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CheckIn()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.CheckInAsync(email);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;

            return StatusCode(StatusCodes.Status201Created, _apiResponse);
        }

        [HttpPost("check-out")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CheckOut()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.CheckOutAsync(email);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetMyAttendance(int? month, int? year)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.GetMyAttendanceAsync(email, month, year);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("employee/{empId}")]
        [HasPermission("Attendance.Read")]
        public async Task<ActionResult<APIResponse>> GetEmployeeAttendance(int empId, int? month, int? year)
        {
            var response = await _attendanceService.GetEmployeeAttendanceAsync(empId, month, year);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("department/{deptId}")]
        [HasPermission("Attendance.Read")]
        public async Task<ActionResult<APIResponse>> GetDepartmentAttendance(int deptId, DateOnly? date)
        {
            var response = await _attendanceService.GetDepartmentAttendanceAsync(deptId, date);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("today")]
        [HasPermission("Attendance.ReadReport")]
        public async Task<ActionResult<APIResponse>> GetTodaySummary()
        {
            var response = await _attendanceService.GetTodaySummaryAsync();

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPut("{id}")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse>> UpdateAttendance(int id, AttendanceUpdateDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var response = await _attendanceService.UpdateAttendanceAsync(id, dto, username);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("report")]
        [HasPermission("Attendance.ReadReport")]
        public async Task<ActionResult<APIResponse>> GetMonthlyReport(int? month, int? year)
        {
            var response = await _attendanceService.GetMonthlyReportAsync(month, year);

            _apiResponse.Data = response;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}
