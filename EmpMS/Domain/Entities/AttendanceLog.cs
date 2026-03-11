using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AttendanceLog
    {
        public int Id { get; set; }
        public int AttendanceId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal? SessionHours { get; set; }
        public DateTime CreatedAt { get; set; }

        public Attendance Attendance { get; set; }
    }
}
