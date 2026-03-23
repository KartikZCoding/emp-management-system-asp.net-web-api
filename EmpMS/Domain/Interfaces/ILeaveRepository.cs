using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ILeaveRepository
    {
        /*Leave Type*/
        Task<List<LeaveType>> GetAllLeaveTypesAsync();
        Task<LeaveType?> GetLeaveTypeByIdAsync(int id);
        Task CreateLeaveTypeAsync(LeaveType leaveType);
        Task UpdateLeaveTypeAsync(LeaveType leaveType);
        Task<bool> LeaveTypeExistsAsync(string name);

        /*Leave Balance*/
        Task<List<LeaveBalance>> GetBalancesByEmployeeAsync(int employeeId, int year);
        Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId, int year);
        Task CreateBalanceAsync(LeaveBalance leaveBalance);
        Task UpdateBalanceAsync(LeaveBalance leaveBalance);
        Task AssignBalancesForEmployeeAsync(int employeeId, int year);

        /*Leave Request*/
        Task<LeaveRequest?> GetRequestByIdAsync(int id);
        Task<List<LeaveRequest>> GetRequestsByEmployeeAsync(int employeeId);
        Task<List<LeaveRequest>> GetPendingRequestAsync();
        Task CreateRequestAsync(LeaveRequest leaveRequest);
        Task UpdateRequestAsync(LeaveRequest leaveRequest);
        Task<bool> HasOverlappingRequestAsync(int employeeId, DateOnly start, DateOnly end);
    }
}
