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
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("check-in")]
        [Authorize]
        public async Task<ActionResult<APIResponse<AttendanceResponseDto>>> CheckIn()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.CheckInAsync(email);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<AttendanceResponseDto>(response)
            {
                StatusCode = HttpStatusCode.Created
            });
        }

        [HttpPost("check-out")]
        [Authorize]
        public async Task<ActionResult<APIResponse<AttendanceResponseDto>>> CheckOut()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.CheckOutAsync(email);
            return Ok(new APIResponse<AttendanceResponseDto>(response));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<APIResponse<List<AttendanceResponseDto>>>> GetMyAttendance(int? month, int? year)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var response = await _attendanceService.GetMyAttendanceAsync(email, month, year);
            return Ok(new APIResponse<List<AttendanceResponseDto>>(response));
        }

        [HttpGet("employee/{empId}")]
        [HasPermission("Attendance.Read")]
        public async Task<ActionResult<APIResponse<List<AttendanceResponseDto>>>> GetEmployeeAttendance(int empId, int? month, int? year)
        {
            var response = await _attendanceService.GetEmployeeAttendanceAsync(empId, month, year);
            return Ok(new APIResponse<List<AttendanceResponseDto>>(response));
        }

        [HttpGet("department/{deptId}")]
        [HasPermission("Attendance.Read")]
        public async Task<ActionResult<APIResponse<List<AttendanceResponseDto>>>> GetDepartmentAttendance(int deptId, DateOnly? date)
        {
            var response = await _attendanceService.GetDepartmentAttendanceAsync(deptId, date);
            return Ok(new APIResponse<List<AttendanceResponseDto>>(response));
        }

        [HttpGet("today")]
        [HasPermission("Attendance.ReadReport")]
        public async Task<ActionResult<APIResponse<TodaySummaryDto>>> GetTodaySummary()
        {
            var response = await _attendanceService.GetTodaySummaryAsync();
            return Ok(new APIResponse<TodaySummaryDto>(response));
        }

        [HttpPut("{id}")]
        [HasPermission("Attendance.Update")]
        public async Task<ActionResult<APIResponse<AttendanceResponseDto>>> UpdateAttendance(int id, AttendanceUpdateDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var response = await _attendanceService.UpdateAttendanceAsync(id, dto, username);
            return Ok(new APIResponse<AttendanceResponseDto>(response));
        }

        [HttpGet("report")]
        [HasPermission("Attendance.ReadReport")]
        public async Task<ActionResult<APIResponse<List<AttendanceReportDto>>>> GetMonthlyReport(int? month, int? year)
        {
            var response = await _attendanceService.GetMonthlyReportAsync(month, year);
            return Ok(new APIResponse<List<AttendanceReportDto>>(response));
        }
    }
}
