using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public  interface IAttendanceRegularizationRepository
    {
        Task<AttendanceRegularization?> GetByIdAsync(int id);
        Task<List<AttendanceRegularization>> GetByEmployeeIdAsync(int employeeId);
        Task<List<AttendanceRegularization>> GetPendingAsync();
        Task CreateAsync(AttendanceRegularization request);
        Task UpdateAsync(AttendanceRegularization request);
        Task<bool> HasPendingRequestAsync(int attendanceId);

    }
}
