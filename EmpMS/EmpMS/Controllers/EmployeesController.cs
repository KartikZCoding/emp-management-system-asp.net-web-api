using Application.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            [FromQuery] int pageSize = 10)
        {
            var result = await _employeeService.GetAllEmployeesAsync(page, pageSize);

            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}
