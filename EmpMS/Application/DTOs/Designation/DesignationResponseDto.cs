namespace Application.DTOs.Designation
{
    public class DesignationResponseDto
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
