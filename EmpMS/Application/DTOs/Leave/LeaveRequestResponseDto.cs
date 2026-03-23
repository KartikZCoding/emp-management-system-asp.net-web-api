using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Leave
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? DecisionNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
