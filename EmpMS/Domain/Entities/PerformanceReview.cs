namespace Domain.Entities
{
    public class PerformanceReview
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }              // FK → Employee being reviewed
        public int ReviewerId { get; set; }              // FK → Employee who wrote the review (Manager/HR)
        public string ReviewPeriod { get; set; }         // e.g., "Q1-2026", "Annual-2025", "H1-2026"
        public int Rating { get; set; }                  // 1 to 5
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string? Comments { get; set; }
        public string? Goals { get; set; }               // Goals for next period
        public DateTime ReviewDate { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }           // The employee being reviewed
        public Employee Reviewer { get; set; }           // The manager/HR who reviewed
    }
}
