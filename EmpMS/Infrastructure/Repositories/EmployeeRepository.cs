using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _appDbContext;
        public EmployeeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Employee>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder)
        {
            var query = _appDbContext.Employees
                .Where(e => e.IsActive)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Manager)
                .AsQueryable();

            query = sortBy?.ToLower() switch
            {
                "name" => sortOrder == "desc"
                            ? query.OrderByDescending(e => e.FirstName)
                            : query.OrderBy(e => e.FirstName),
                "salary" => sortOrder == "desc"
                            ? query.OrderByDescending(e => e.Salary)
                            : query.OrderBy(e => e.Salary),
                "joindate" => sortOrder == "desc"
                            ? query.OrderByDescending(e => e.JoinDate)
                            : query.OrderBy(e => e.JoinDate),
                "department" => sortOrder == "desc"
                            ? query.OrderByDescending(e => e.Department.DepartmentName)
                            : query.OrderBy(e => e.Department.DepartmentName),
                _ => query.OrderBy(e => e.Id) // default sort by Id
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalCountAsync()
        {
            return await _appDbContext.Employees.Where(e => e.IsActive).CountAsync();
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _appDbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Manager)
                .Where(e => e.Id == id && e.IsActive)
                .FirstOrDefaultAsync();
        }
        public async Task CreateAsync(Employee employee)
        {
            await _appDbContext.Employees.AddAsync(employee);

        }
        public async Task UpdateAsync(Employee employee)
        {
            _appDbContext.Employees.Update(employee);

        }
        public async Task SoftDeleteAsync(Employee employee)
        {
            employee.IsActive = false;
            employee.UpdatedAt = DateTime.Now;
            _appDbContext.Employees.Update(employee);

        }
        public async Task<List<Employee>> SearchAsync(string? name, int? deptId, int? designationId)
        {
            var query = _appDbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
            if (deptId.HasValue)
                query = query.Where(e => e.DepartmentId == deptId.Value);
            if (designationId.HasValue)
                query = query.Where(e => e.DesignationId == designationId.Value);

            return await query.ToListAsync();
        }
        public async Task<List<Employee>> GetByDepartmentAsync(int departmentId)
        {
            return await _appDbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .ToListAsync();
        }
        public async Task<List<Employee>> GetByManagerAsync(int managerId)
        {
            return await _appDbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.ManagerId == managerId && e.IsActive)
                .ToListAsync();
        }
        public async Task<bool> EmailExistAsync(string email)
        {
            return await _appDbContext.Employees.AnyAsync(e => e.Email == email && e.IsActive);
        }
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _appDbContext.Employees
                .Include(e => e.Department).Include(e => e.Designation).Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
        }

        public async Task UpdatePhotoPathAsync(int id, string photoPath)
        {
            var employee = await _appDbContext.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.PhotoPath = photoPath;
                employee.UpdatedAt = DateTime.Now;
    
            }
        }
    }
}
