using Application.DTOs.Attendance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAttendanceRegularizationService
    {
        Task<AttendanceRegularizationResponseDto> CreateRequestAsync(string employeeEmail, AttendanceRegularizationRequestDto dto);
        Task<List<AttendanceRegularizationResponseDto>> GetMyRequestsAsync(string employeeEmail);
        Task<List<AttendanceRegularizationResponseDto>> GetPendingRequestsAsync();
        Task<AttendanceRegularizationResponseDto> ApproveAsync(int requestId, int hrUserId, string? decisionNote);
        Task<AttendanceRegularizationResponseDto> RejectAsync(int requestId, int hrUserId, string? decisionNote);
    }
}
