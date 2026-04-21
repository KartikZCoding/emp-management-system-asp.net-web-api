using Application.Common;
using Application.DTOs.Dashboard;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [HasPermission("Dashboard.View")]
        public async Task<ActionResult<APIResponse<DashboardSummaryDto>>> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();
            return Ok(new APIResponse<DashboardSummaryDto>(result));
        }

        [HttpGet("attendance-overview")]
        [HasPermission("Dashboard.View")]
        public async Task<ActionResult<APIResponse<AttendanceOverviewDto>>> GetAttendanceOverview([FromQuery] int month, [FromQuery] int year)
        {
            var result = await _dashboardService.GetAttendanceOverviewAsync(month, year);
            return Ok(new APIResponse<AttendanceOverviewDto>(result));
        }

        [HttpGet("department-stats")]
        [HasPermission("Dashboard.View")]
        public async Task<ActionResult<APIResponse<DepartmentStatsDto>>> GetDepartmentStats()
        {
            var result = await _dashboardService.GetDepartmentStatsAsync();
            return Ok(new APIResponse<DepartmentStatsDto>(result));
        }

        [HttpGet("leave-stats")]
        [HasPermission("Dashboard.View")]
        public async Task<ActionResult<APIResponse<LeaveStatsDto>>> GetLeaveStats([FromQuery] int year)
        {
            var result = await _dashboardService.GetLeaveStatsAsync(year);
            return Ok(new APIResponse<LeaveStatsDto>(result));
        }

        [HttpGet("salary-stats")]
        [HasPermission("Report.Salary")]
        public async Task<ActionResult<APIResponse<SalaryStatsDto>>> GetSalaryStats([FromQuery] int year)
        {
            var result = await _dashboardService.GetSalaryStatsAsync(year);
            return Ok(new APIResponse<SalaryStatsDto>(result));
        }
    }
}
