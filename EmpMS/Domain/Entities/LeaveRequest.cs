using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int ApprovedById { get; set; }
        public DateTime DecisionDate { get; set; }
        public string DesicionNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
        public User ApprovedBy { get; set; }
    }
}
