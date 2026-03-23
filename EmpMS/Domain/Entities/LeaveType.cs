using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DefaultDays { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public ICollection<LeaveBalance> leaveBalances { get; set; }
        public ICollection<LeaveRequest> leaveRequests { get; set; }
    }
}
