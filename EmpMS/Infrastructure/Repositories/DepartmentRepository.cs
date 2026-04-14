using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _appDbContext;

        public DepartmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _appDbContext.Departments.AsNoTracking().Where(d => d.IsActive).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _appDbContext.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }

        public async Task<bool> ExistsAsync(string departmentName)
        {
            return await _appDbContext.Departments.AnyAsync(d => d.DepartmentName == departmentName && d.IsActive);
        }

        public async Task CreateAsync(Department department)
        {
            await _appDbContext.Departments.AddAsync(department);

        }

        public async Task UpdateAsync(Department department)
        {
            _appDbContext.Departments.Update(department);

        }

        public async Task DeleteAsync(Department department)
        {
            _appDbContext.Departments.Update(department);

        }

        public async Task<List<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
            return await _appDbContext.Employees
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
