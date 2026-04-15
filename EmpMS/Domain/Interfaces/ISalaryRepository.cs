using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ISalaryRepository
    {
        Task<Salary?> GetByIdAsync(int id);
        Task<Salary?> GetByEmployeeMonthYearAsync(int employeeId, int month, int year);
        Task<List<Salary>> GetByEmployeeAsync(int employeeId, int? month, int? year);
        Task<List<Salary>> GetAllByMonthYearAsync(int month, int year);
        Task<List<Salary>> GetYearlyAllAsync(int year);
        Task<bool> ExistsAsync(int employeeId, int month, int year);
        Task AddAsync(Salary salary);
        Task AddRangeAsync(List<Salary> salaries);
        void Update(Salary salary);
    }
}
