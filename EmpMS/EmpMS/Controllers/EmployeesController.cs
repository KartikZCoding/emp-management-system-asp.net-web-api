using Application.Common;
using Application.DTOs.Employee;
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
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<PaginatedResult<EmployeeListDto>>>> GetAllEmployees(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc")
        {
            var result = await _employeeService.GetAllEmployeesAsync(page, pageSize, sortBy, sortOrder);
            return Ok(new APIResponse<PaginatedResult<EmployeeListDto>>(result));
        }

        [HttpGet("{id}")]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<EmployeeResponseDto>>> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(new APIResponse<EmployeeResponseDto>(result));
        }

        [HttpPost]
        [HasPermission("Employee.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateEmployee(CreateEmployeeDto dto)
        {
            await _employeeService.CreateEmployeeAsync(dto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Employee created successfully"
            });
        }

        [HttpPut("{id}")]
        [HasPermission("Employee.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            await _employeeService.UpdateEmployeeAsync(id, dto);

            return Ok(new APIResponse { Message = "Employee updated successfully" });
        }

        [HttpDelete("{id}")]
        [HasPermission("Employee.Delete")]// only Admin can delete
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteEmployee(int id)
        {
            await _employeeService.SoftDeleteEmployeeAsync(id);

            return Ok(new APIResponse { Message = "Employee deleted successfully" });
        }

        [HttpGet("search")]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<List<EmployeeListDto>>>> SearchEmployees(
            [FromQuery] string? name,
            [FromQuery] int? dept,
            [FromQuery] int? designation)
        {
            var result = await _employeeService.SearchEmployeesAsync(name, dept, designation);
            return Ok(new APIResponse<List<EmployeeListDto>>(result));
        }

        [HttpGet("department/{deptId}")]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<List<EmployeeListDto>>>> GetByDepartment(int deptId)
        {
            var result = await _employeeService.GetByDepartmentAsync(deptId);
            return Ok(new APIResponse<List<EmployeeListDto>>(result));
        }

        [HttpGet("manager/{managerId}")]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<List<EmployeeListDto>>>> GetByManager(int managerId)
        {
            var result = await _employeeService.GetByManagerAsync(managerId);
            return Ok(new APIResponse<List<EmployeeListDto>>(result));
        }

        [HttpGet("me")]
        [Authorize]// any authenticated user
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<EmployeeResponseDto>>> GetOwnProfile()
        {
            // get current user's email from JWT token claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var result = await _employeeService.GetOwnProfileAsync(email);
            return Ok(new APIResponse<EmployeeResponseDto>(result));
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

            return Ok(new APIResponse { Message = "Profile updated successfully" });
        }

        [HttpPost("{id}/photo")]
        [HasPermission("Employee.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UploadPhoto(int id, IFormFile file)
        {
            //_logger.LogInformation("Controller : called service");
            await _employeeService.UploadPhotoAsync(id, file);

            return Ok(new APIResponse { Message = "Photo uploaded successfully" });
        }

        [HttpGet("{id}/photo")]
        [HasPermission("Employee.Read")]// photos can be viewed without login
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var (fileBytes, contentType) = await _employeeService.GetPhotoAsync(id);
            return File(fileBytes, contentType);
        }
    }
}
