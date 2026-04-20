namespace Application.DTOs.Review
{
    public class UpdateReviewDto
    {
        public int? Rating { get; set; }                 // 1 to 5 (optional — only update if provided)
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Comments { get; set; }
        public string? Goals { get; set; }
    }
}
