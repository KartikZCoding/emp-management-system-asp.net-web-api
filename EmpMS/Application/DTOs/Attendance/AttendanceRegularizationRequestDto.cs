using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Attendance
{
    public class AttendanceRegularizationRequestDto
    {
        public DateOnly Date { get; set; }              // Employee enters the date of missed checkout
        public TimeOnly RequestedCheckOut { get; set; } // Employee enters just the time, e.g. 18:30
        public string Note { get; set; }
    }
}
