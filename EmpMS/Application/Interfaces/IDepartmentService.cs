using Application.DTOs.Department;
using Application.DTOs.Employee;

namespace Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync();
        Task<DepartmentResponseDto> GetDepartmentByIdAsync(int id);
        Task CreateDepartmentAsync(DepartmentDto departmentDto);
        Task UpdateDepartmentAsync(int id, DepartmentDto departmentDto);
        Task DeleteDepartmentAsync(int id);
        Task<List<EmployeeListDto>> GetEmployeesInDepartmentAsync(int departmentId);
    }
}
