using Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<AttendanceOverviewDto> GetAttendanceOverviewAsync(int month, int year);
        Task<DepartmentStatsDto> GetDepartmentStatsAsync();
        Task<LeaveStatsDto> GetLeaveStatsAsync(int year);
        Task<SalaryStatsDto> GetSalaryStatsAsync(int year);
    }
}
