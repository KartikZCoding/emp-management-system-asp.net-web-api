using System.Collections.Generic;

namespace Application.DTOs.Dashboard
{
    public class LeaveStatsDto
    {
        public int Year { get; set; }
        public int TotalLeaveRequests { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
        public List<LeaveTypeBreakdownDto> LeaveTypeBreakdown { get; set; } = new();
    }

    public class LeaveTypeBreakdownDto
    {
        public string LeaveType { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
