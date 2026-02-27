using Application.Common;
using Application.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<PaginatedResult<EmployeeListDto>> GetAllEmployeesAsync(int page, int pageSize);
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(CreateEmployeeDto dto);
        Task UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
        Task SoftDeleteEmployeeAsync(int id);
        Task<List<EmployeeListDto>> SearchEmployeesAsync(string? name, int? deptId, int? designationId);
        Task<List<EmployeeListDto>> GetByDepartmentAsync(int deptId);
        Task<List<EmployeeListDto>> GetByManagerAsync(int managerId);
        Task<EmployeeResponseDto> GetOwnProfileAsync(string email);
        Task UpdateOwnProfileAsync(string email, UpdateOwnProfileDto dto);
    }
}
