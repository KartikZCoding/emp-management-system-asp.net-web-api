using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class TodaySummaryDto
    {
        public DateOnly Date { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalCheckedIn { get; set; }
        public int TotalPresent { get; set; }
        public int TotalLate { get; set; }
        public int CurrentlyInOffice { get; set; }
        public int TotalAbsent { get; set; }
    }
}
