namespace Application.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewPeriod { get; set; }
        public int Rating { get; set; }
        public string RatingLabel { get; set; }          // "Outstanding", "Exceeds Expectations", etc.
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string? Comments { get; set; }
        public string? Goals { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
