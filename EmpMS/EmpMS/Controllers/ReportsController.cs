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
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("employees")]
        [HasPermission("Report.Employees")]
        public async Task<IActionResult> ExportEmployees()
        {
            var csvBytes = await _reportService.GenerateEmployeesReportCsvAsync();
            return File(csvBytes, "text/csv", "EmployeesReport.csv");
        }

        [HttpGet("attendance")]
        [HasPermission("Report.Attendance")]
        public async Task<IActionResult> ExportAttendance([FromQuery] int month, [FromQuery] int year)
        {
            var csvBytes = await _reportService.GenerateAttendanceReportCsvAsync(month, year);
            return File(csvBytes, "text/csv", $"AttendanceReport_{month}_{year}.csv");
        }

        [HttpGet("salary")]
        [HasPermission("Report.Salary")]
        public async Task<IActionResult> ExportSalary([FromQuery] int month, [FromQuery] int year)
        {
            var csvBytes = await _reportService.GenerateSalaryReportCsvAsync(month, year);
            return File(csvBytes, "text/csv", $"SalaryReport_{month}_{year}.csv");
        }
    }
}
