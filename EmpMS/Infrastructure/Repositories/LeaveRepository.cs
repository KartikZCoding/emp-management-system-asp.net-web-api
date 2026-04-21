using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _appDbContext;

        public LeaveRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<LeaveType>> GetAllLeaveTypesAsync()
        {
            return await _appDbContext.LeaveTypes
                .Where(lt => lt.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveType?> GetLeaveTypeByIdAsync(int id)
        {
            return await _appDbContext.LeaveTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.Id == id);
        }

        public async Task CreateLeaveTypeAsync(LeaveType leaveType)
        {
            await _appDbContext.LeaveTypes.AddAsync(leaveType);

        }

        public async Task UpdateLeaveTypeAsync(LeaveType leaveType)
        {
            _appDbContext.LeaveTypes.Update(leaveType);

        }

        public async Task<bool> LeaveTypeExistsAsync(string name)
        {
            return await _appDbContext.LeaveTypes
                .AnyAsync(lt => lt.Name == name && lt.IsActive);
        }

        public async Task<List<LeaveBalance>> GetBalancesByEmployeeAsync(int employeeId, int year)
        {
            return await _appDbContext.LeaveBalances
                .Include(lb => lb.LeaveType)
                .Where(lb => lb.EmployeeId == employeeId && lb.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId, int year)
        {
            return await _appDbContext.LeaveBalances
                .AsNoTracking()
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId
                    && lb.LeaveTypeId == leaveTypeId
                    && lb.Year == year);
        }

        public async Task CreateBalanceAsync(LeaveBalance leaveBalance)
        {
            await _appDbContext.LeaveBalances.AddAsync(leaveBalance);

        }

        public async Task UpdateBalanceAsync(LeaveBalance leaveBalance)
        {
            _appDbContext.LeaveBalances.Update(leaveBalance);

        }

        public async Task AssignBalancesForEmployeeAsync(int employeeId, int year)
        {
            var leaveTypes = await _appDbContext.LeaveTypes
                .Where(lt => lt.IsActive)
                .AsNoTracking()
                .ToListAsync();

            var balances = leaveTypes.Select(lt => new LeaveBalance
            {
                EmployeeId = employeeId,
                LeaveTypeId = lt.Id,
                Year = year,
                TotalLeaves = lt.DefaultDays,
                UsedLeaves = 0,
                RemainingLeaves = lt.DefaultDays
            }).ToList();

            await _appDbContext.LeaveBalances.AddRangeAsync(balances);

        }

        public async Task<List<LeaveRequest>> GetAllRequestsAsync()
        {
            return await _appDbContext.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetRequestByIdAsync(int id)
        {
            return await _appDbContext.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .AsNoTracking()
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<List<LeaveRequest>> GetRequestsByEmployeeAsync(int employeeId)
        {
            return await _appDbContext.LeaveRequests
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.EmployeeId == employeeId)
                .OrderByDescending(lr => lr.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetPendingRequestAsync()
        {
            return await _appDbContext.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.Status == "Pending")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateRequestAsync(LeaveRequest leaveRequest)
        {
            await _appDbContext.LeaveRequests.AddAsync(leaveRequest);

        }

        public async Task UpdateRequestAsync(LeaveRequest leaveRequest)
        {
            _appDbContext.LeaveRequests.Update(leaveRequest);

        }

        public async Task<bool> HasOverlappingRequestAsync(int employeeId, DateOnly start, DateOnly end)
        {
            return await _appDbContext.LeaveRequests
                .AnyAsync(lr => lr.EmployeeId == employeeId
                    && lr.Status != "Rejected"
                    && lr.Status != "Cancelled"
                    && lr.StartDate <= end
                    && lr.EndDate >= start);
        }
    }
}
