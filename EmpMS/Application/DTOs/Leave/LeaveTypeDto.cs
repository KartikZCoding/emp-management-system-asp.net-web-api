using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Leave
{
    public class LeaveTypeDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DefaultDays { get; set; }
        public bool IsPaid { get; set; }

    }
}
