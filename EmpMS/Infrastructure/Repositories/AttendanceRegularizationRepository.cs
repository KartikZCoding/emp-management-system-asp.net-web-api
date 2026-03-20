using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class AttendanceRegularizationRepository : IAttendanceRegularizationRepository
    {
        private readonly AppDbContext _appDbContext;

        public AttendanceRegularizationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AttendanceRegularization?> GetByIdAsync(int id)
        {
            return await _appDbContext.AttendanceRegularizations
                .Include(e => e.Employee)
                .Include(e => e.Attendance.AttendanceLogs)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<AttendanceRegularization>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _appDbContext.AttendanceRegularizations
                .Include(e => e.Employee)
                .Include(e => e.Attendance.AttendanceLogs)
                .Where(e => e.EmployeeId == employeeId).OrderByDescending(e => e.CreatedAt).ToListAsync();
        }

        public async Task<List<AttendanceRegularization>> GetPendingAsync()
        {
            return await _appDbContext.AttendanceRegularizations
                .Include(e => e.Employee)
                .Where(e => e.Status == "Pending").ToListAsync();
        }

        public async Task CreateAsync(AttendanceRegularization request)
        {
            await _appDbContext.AttendanceRegularizations.AddAsync(request);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AttendanceRegularization request)
        {
            _appDbContext.AttendanceRegularizations.Update(request);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> HasPendingRequestAsync(int attendanceId)
        {
            return await _appDbContext.AttendanceRegularizations.AnyAsync(e => e.AttendanceId == attendanceId && e.Status == "Pending");
        }
    }
}
