using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public int TotalLeaves { get; set; }
        public int UsedLeaves { get; set; }
        public int RemainingLeaves { get; set; }

        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
