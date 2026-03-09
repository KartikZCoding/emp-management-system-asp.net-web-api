using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(string departmentName);
        Task CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(Department department);
        Task<List<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId);
    }
}
