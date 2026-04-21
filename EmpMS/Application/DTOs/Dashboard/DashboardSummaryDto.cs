namespace Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalDesignations { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int TodayPresentCount { get; set; }
        public int TodayAbsentCount { get; set; }
        public double AverageAttendancePercent { get; set; }
        public decimal CurrentMonthSalaryExpenditure { get; set; }
    }
}
