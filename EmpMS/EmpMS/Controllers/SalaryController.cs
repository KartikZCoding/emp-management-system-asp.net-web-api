using Application.Common;
using Application.DTOs.Salary;
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
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpPost("generate")]
        [HasPermission("Salary.Create")]
        public async Task<ActionResult<APIResponse<List<SalaryResponseDto>>>> GenerateSalary(
            [FromQuery] int month, [FromQuery] int year)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var response = await _salaryService.GenerateMonthlySalaryAsync(month, year, username);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<List<SalaryResponseDto>>(response)
            {
                StatusCode = HttpStatusCode.Created,
                Message = $"Salary generated successfully for {response.Count} employees"
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<APIResponse<SalaryResponseDto>>> GetMySalary(
            [FromQuery] int month, [FromQuery] int year)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _salaryService.GetMySalaryAsync(email, month, year);
            return Ok(new APIResponse<SalaryResponseDto>(response));
        }

        [HttpGet("employee/{empId}")]
        [HasPermission("Salary.Read")]
        public async Task<ActionResult<APIResponse<List<SalaryResponseDto>>>> GetEmployeeSalary(
            int empId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var response = await _salaryService.GetEmployeeSalaryAsync(empId, month, year);
            return Ok(new APIResponse<List<SalaryResponseDto>>(response));
        }

        [HttpGet("all")]
        [HasPermission("Salary.Read")]
        public async Task<ActionResult<APIResponse<List<SalaryResponseDto>>>> GetAllSalaries(
            [FromQuery] int month, [FromQuery] int year)
        {
            var response = await _salaryService.GetAllSalariesAsync(month, year);
            return Ok(new APIResponse<List<SalaryResponseDto>>(response));
        }

        [HttpPut("{id}")]
        [HasPermission("Salary.Update")]
        public async Task<ActionResult<APIResponse<SalaryResponseDto>>> UpdateSalary(
            int id, SalaryUpdateDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var response = await _salaryService.UpdateSalaryAsync(id, dto, username);
            return Ok(new APIResponse<SalaryResponseDto>(response));
        }

        [HttpGet("report")]
        [HasPermission("Salary.Read")]
        public async Task<ActionResult<APIResponse<List<SalaryReportDto>>>> GetYearlyReport(
            [FromQuery] int year)
        {
            var response = await _salaryService.GetYearlySalaryReportAsync(year);
            return Ok(new APIResponse<List<SalaryReportDto>>(response));
        }
    }
}
