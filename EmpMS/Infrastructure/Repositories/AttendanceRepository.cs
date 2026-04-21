using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _appDbContext;

        public AttendanceRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<Attendance?> GetByEmployeeAndDateAsync(int empId, DateOnly date)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.EmployeeId == empId && a.Date == date);
        }
        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Attendance>> GetByEmployeeMonthlyAsync(int empId, int month, int year)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.EmployeeId == empId && a.Date.Month == month && a.Date.Year == year)
                .OrderByDescending(a => a.Date)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Attendance>> GetByDepartmentAndDateAsync(int deptId, DateOnly date)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.Employee.DepartmentId == deptId && a.Date == date)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Attendance>> GetTodayAsync(DateOnly today)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.Date == today)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Attendance>> GetMonthlyAllAsync(int month, int year)
        {
            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.Date.Month == month && a.Date.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateAsync(Attendance attendance)
        {
            await _appDbContext.Attendances.AddAsync(attendance);

        }
        public async Task CreateLogAsync(AttendanceLog log)
        {
            await _appDbContext.AttendanceLogs.AddAsync(log);

        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _appDbContext.Attendances.Update(attendance);

        }
        public async Task UpdateLogAsync(AttendanceLog log)
        {
            _appDbContext.AttendanceLogs.Update(log);

        }

        public async Task<int> GetActiveEmployeeCountAsync()
        {
            return await _appDbContext.Employees.CountAsync(e => e.IsActive);
        }

        public async Task<List<Attendance>> GetMissedCheckoutsAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _appDbContext.Attendances
                .Include(a => a.AttendanceLogs)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.EmployeeId == employeeId
                       && a.Date < today   // exclude today — employee is still working!
                       && a.AttendanceLogs.Any(l => l.CheckOut == null))
                .OrderByDescending(a => a.Date)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
