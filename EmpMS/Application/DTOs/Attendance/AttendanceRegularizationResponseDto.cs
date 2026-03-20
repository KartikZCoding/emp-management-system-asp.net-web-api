using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class AttendanceRegularizationResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequestedCheckOut { get; set; }
        public string Note { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public int? HRorAdminId { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string DecisionNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
