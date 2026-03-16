using Application.DTOs.Attendance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDto> CheckInAsync(string email);
        Task<AttendanceResponseDto> CheckOutAsync(string email);
        Task<List<AttendanceResponseDto>> GetMyAttendanceAsync(string email, int? month, int? year);
        Task<List<AttendanceResponseDto>> GetEmployeeAttendanceAsync(int empId, int? month, int? year);
        Task<List<AttendanceResponseDto>> GetDepartmentAttendanceAsync(int deptId, DateOnly? date);
        Task<TodaySummaryDto> GetTodaySummaryAsync();
        Task<AttendanceResponseDto> UpdateAttendanceAsync(int id, AttendanceUpdateDto dto, string updateBy);
        Task<List<AttendanceReportDto>> GetMonthlyReportAsync(int? month, int? year);

    }
}
