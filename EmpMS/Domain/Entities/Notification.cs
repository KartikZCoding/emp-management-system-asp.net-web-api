using System;

namespace Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // e.g. "Leave", "Salary", "Review", "Broadcast", "System"
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public Employee Employee { get; set; }
    }
}
