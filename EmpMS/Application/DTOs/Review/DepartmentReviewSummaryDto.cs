namespace Application.DTOs.Review
{
    public class DepartmentReviewSummaryDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Year { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalReviewsConducted { get; set; }
        public int EmployeesNotReviewed { get; set; }
        public decimal AverageRating { get; set; }
        public RatingDistributionDto RatingDistribution { get; set; }
    }

    public class RatingDistributionDto
    {
        public int Outstanding { get; set; }             // Rating 5
        public int ExceedsExpectations { get; set; }     // Rating 4
        public int MeetsExpectations { get; set; }       // Rating 3
        public int NeedsImprovement { get; set; }        // Rating 2
        public int Unsatisfactory { get; set; }          // Rating 1
    }
}
