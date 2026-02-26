namespace Domain.Entities
{
    public class Department
    {
        //table prop
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        //audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //navigation prop
        public ICollection<Employee> Employees { get; set; } //one department has many employees
    }
}
