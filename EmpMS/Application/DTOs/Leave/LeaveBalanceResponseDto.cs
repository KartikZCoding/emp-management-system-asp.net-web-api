using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Leave
{
    public class LeaveBalanceResponseDto
    {
        public int Id { get; set; }
        public string LeaveTypeName { get; set; }
        public int Year { get; set; }
        public int TotalLeaves { get; set; }
        public int UsedLeaves { get; set; }
        public int RemainingLeaves { get; set; }

    }
}
