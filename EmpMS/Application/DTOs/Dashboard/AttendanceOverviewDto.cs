using System.Collections.Generic;

namespace Application.DTOs.Dashboard
{
    public class AttendanceOverviewDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalWorkingDays { get; set; }
        public double AveragePresentDays { get; set; }
        public double AverageAbsentDays { get; set; }
        public int LateCheckInCount { get; set; }
        public int MissedCheckoutCount { get; set; }
        public List<DepartmentAttendanceDto> DepartmentWiseAttendance { get; set; } = new();
    }

    public class DepartmentAttendanceDto
    {
        public string DepartmentName { get; set; }
        public double PresentPercent { get; set; }
    }
}
