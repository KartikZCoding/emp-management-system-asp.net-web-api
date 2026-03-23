using Application.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ILeaveService
    {
        Task<List<LeaveTypeResponseDto>> GetAllLeaveTypesAsync();
        Task<LeaveTypeResponseDto> GetLeaveTypeByIdAsync(int id);
        Task<LeaveTypeResponseDto> CreateLeaveTypeAsync(LeaveTypeDto dto);
        Task<LeaveTypeResponseDto> UpdateLeaveTypeAsync(int id, LeaveTypeDto dto);
        Task DeleteLeaveTypeAsync(int id);

        Task<List<LeaveBalanceResponseDto>> GetMyBalancesAsync(string employeeEmail, int year);
        Task AssignBalancesAsync(int employeeId, int year);

        Task<LeaveRequestResponseDto> ApplyLeaveAsync(string employeeEmail, LeaveRequestDto dto);
        Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(string employeeEmail);
        Task<List<LeaveRequestResponseDto>> GetPendingRequestAsync();
        Task<LeaveRequestResponseDto> ApproveLeaveAsync(int requestId, int hrUserId, string? decisionNote);
        Task<LeaveRequestResponseDto> RejectLeaveAsync(int requestId, int hrUserId, string? decisionNote);
        Task<LeaveRequestResponseDto> CancelLeaveAsync(string employeeEmail, int requestId);
    }
}
