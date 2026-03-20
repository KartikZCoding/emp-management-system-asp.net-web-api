using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class AttendanceRegularizationRequestDto
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequestedCheckOut { get; set; }
        public string Note { get; set; }
    }
}
