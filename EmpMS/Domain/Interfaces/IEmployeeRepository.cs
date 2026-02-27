using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {   
        Task<List<Employee>> GetAllAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task SoftDeleteAsync(Employee employee);
        Task<List<Employee>> SearchAsync(string? name, int? deptId, int? designationId);
        Task<List<Employee>> GetByDepartmentAsync(int departmentId);
        Task<List<Employee>> GetByManagerAsync(int managerId);
        Task<bool> EmailExistAsync(string email);
        Task<Employee?> GetByEmailAsync(string email);
    }   
}
