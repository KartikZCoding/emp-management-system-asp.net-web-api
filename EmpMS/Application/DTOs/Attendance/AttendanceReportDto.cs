using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class AttendanceReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalPresentDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public int TotalLateDays { get; set; }
        public int TotalHalfDays { get; set; }
        public int TotalOnLeaveDays { get; set; }
        public decimal AverageWorkHours { get; set; }
        public decimal TotalWorkhours { get; set; }
    }
}
