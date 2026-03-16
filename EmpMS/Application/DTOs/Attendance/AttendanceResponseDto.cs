using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class AttendanceResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly Date { get; set; }
        public decimal? TotalHours { get; set; }
        public string Status { get; set; }
        public bool IsLate { get; set; }
        public bool IsCheckedIn { get; set; }
        public List<AttendanceLogResponseDto> Logs { get; set; }
    }
}
