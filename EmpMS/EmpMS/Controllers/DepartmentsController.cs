using Application.Common;
using Application.DTOs.Department;
using Application.DTOs.Employee;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [HasPermission("Department.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<DepartmentResponseDto>>>> GetAllDepartments()
        {
            var response = await _departmentService.GetAllDepartmentsAsync();
            return Ok(new APIResponse<List<DepartmentResponseDto>>(response));
        }

        [HttpGet("{id}")]
        [HasPermission("Department.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<DepartmentResponseDto>>> GetDepartmentById(int id)
        {
            var response = await _departmentService.GetDepartmentByIdAsync(id);
            return Ok(new APIResponse<DepartmentResponseDto>(response));
        }

        [HttpPost]
        [HasPermission("Department.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateDepartment(DepartmentDto departmentDto)
        {
            await _departmentService.CreateDepartmentAsync(departmentDto);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Department created successfully"
            });
        }

        [HttpPut("{id}")]
        [HasPermission("Department.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateDepartment(int id, DepartmentDto departmentDto)
        {
            await _departmentService.UpdateDepartmentAsync(id, departmentDto);

            return Ok(new APIResponse { Message = "Department updated successfully" });
        }

        [HttpDelete("{id}")]
        [HasPermission("Department.Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);

            return Ok(new APIResponse { Message = "Department deleted successfully" });
        }

        [HttpGet("{id}/employees")]
        [HasPermission("Employee.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<EmployeeListDto>>>> GetEmployeesInDepartment(int id)
        {
            var response = await _departmentService.GetEmployeesInDepartmentAsync(id);
            return Ok(new APIResponse<List<EmployeeListDto>>(response));
        }
    }
}
