using Application.Common;
using Application.DTOs.Employee;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private APIResponse _apiResponse;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _apiResponse = new();
        }

        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllEmployees(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc")
        {
            var result = await _employeeService.GetAllEmployeesAsync(page, pageSize, sortBy, sortOrder);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "HR,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Authorize(Roles = "HR,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> CreateEmployee(CreateEmployeeDto dto)
        {
            await _employeeService.CreateEmployeeAsync(dto);

            _apiResponse.Data = "Employee created successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            await _employeeService.UpdateEmployeeAsync(id, dto);

            _apiResponse.Data = "Employee updated successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]       // only Admin can delete
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteEmployee(int id)
        {
            await _employeeService.SoftDeleteEmployeeAsync(id);

            _apiResponse.Data = "Employee deleted successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("search")]
        [Authorize(Roles = "HR,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> SearchEmployees(
            [FromQuery] string? name,
            [FromQuery] int? dept,
            [FromQuery] int? designation)
        {
            var result = await _employeeService.SearchEmployeesAsync(name, dept, designation);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("department/{deptId}")]
        [Authorize(Roles = "HR,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetByDepartment(int deptId)
        {
            var result = await _employeeService.GetByDepartmentAsync(deptId);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("manager/{managerId}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetByManager(int managerId)
        {
            var result = await _employeeService.GetByManagerAsync(managerId);
            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("me")]
        [Authorize]     // any authenticated user
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetOwnProfile()
        {
            // get current user's email from JWT token claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var result = await _employeeService.GetOwnProfileAsync(email);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateOwnProfile(UpdateOwnProfileDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();
            await _employeeService.UpdateOwnProfileAsync(email, dto);
            _apiResponse.Data = "Profile updated successfully!";
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
    }
}
