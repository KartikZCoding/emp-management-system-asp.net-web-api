using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly AppDbContext _appDbContext;
        public SalaryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Salary?> GetByIdAsync(int id)
        {
            return await _appDbContext.Salaries
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Department)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Designation)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Salary?> GetByEmployeeMonthYearAsync(int employeeId, int month, int year)
        {
            return await _appDbContext.Salaries
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Department)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Designation)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId
                                       && s.Month == month
                                       && s.Year == year);
        }

        public async Task<List<Salary>> GetByEmployeeAsync(int employeeId, int? month, int? year)
        {
            var query = _appDbContext.Salaries
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Department)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Designation)
                .Where(s => s.EmployeeId == employeeId);

            if (month.HasValue)
                query = query.Where(s => s.Month == month.Value);

            if (year.HasValue)
                query = query.Where(s => s.Year == year.Value);

            return await query
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Salary>> GetAllByMonthYearAsync(int month, int year)
        {
            return await _appDbContext.Salaries
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Department)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Designation)
                .Where(s => s.Month == month && s.Year == year)
                .OrderBy(s => s.Employee.FirstName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Salary>> GetYearlyAllAsync(int year)
        {
            return await _appDbContext.Salaries
                .Include(s => s.Employee)
                    .ThenInclude(e => e.Department)
                .Where(s => s.Year == year)
                .OrderBy(s => s.Month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int employeeId, int month, int year)
        {
            return await _appDbContext.Salaries
                .AnyAsync(s => s.EmployeeId == employeeId
                            && s.Month == month
                            && s.Year == year);
        }

        public async Task AddAsync(Salary salary)
        {
            await _appDbContext.Salaries.AddAsync(salary);
        }

        public async Task AddRangeAsync(List<Salary> salaries)
        {
            await _appDbContext.Salaries.AddRangeAsync(salaries);
        }

        public void Update(Salary salary)
        {
            _appDbContext.Salaries.Update(salary);
        }
    }
}
