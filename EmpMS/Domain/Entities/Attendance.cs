using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public decimal? TotalHours { get; set; }
        public bool IsLate { get; set; }
        public string Status { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public Employee Employee { get; set; }
        public ICollection<AttendanceLog> AttendanceLogs { get; set; }
    }
}
