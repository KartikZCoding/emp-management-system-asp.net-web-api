using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetByEmployeeAndDateAsync(int empId, DateOnly date);
        Task<Attendance?> GetByIdAsync(int id);
        Task<List<Attendance>> GetByEmployeeMonthlyAsync(int empId, int month, int year);
        Task<List<Attendance>> GetByDepartmentAndDateAsync(int deptId, DateOnly date);
        Task<List<Attendance>> GetTodayAsync(DateOnly today);
        Task<List<Attendance>> GetMonthlyAllAsync(int month, int year);
        Task CreateAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task CreateLogAsync(AttendanceLog log);
        Task UpdateLogAsync(AttendanceLog log);
        Task<int> GetActiveEmployeeCountAsync();

    }
}
