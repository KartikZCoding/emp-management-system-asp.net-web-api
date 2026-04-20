namespace Application.DTOs.Review
{
    public class CreateReviewDto
    {
        public int EmployeeId { get; set; }
        public string ReviewPeriod { get; set; }         // e.g., "Q1-2026", "Annual-2025"
        public int Rating { get; set; }                  // 1 to 5
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string? Comments { get; set; }
        public string? Goals { get; set; }
    }
}
