using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Leave
{
    public class LeaveRequestDto
    {
        public int LeaveTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; }
    }
}
