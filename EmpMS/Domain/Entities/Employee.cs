namespace Domain.Entities
{
    public class Employee
    {
        //table prop
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
        public int DepartmentId { get; set; } //FK -> Departments
        public int DesignationId { get; set; } //FK -> Designations
        public int? ManagerId { get; set; } //FK -> self (nullable, not everyone has a manager)
        public decimal Salary { get; set; }
        public string? PhotoPath { get; set; } //nullable — photo may not be uploaded yet
        public bool IsActive { get; set; }

        //audit fields
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        //navigation prop
        public Department Department { get; set; } //each employee belongs to one department
        public Designation Designation { get; set; } //each employee has one designation
        public Employee? Manager { get; set; } //each employee may have one manager
        public ICollection<Employee> Subordinates { get; set; } //a manager can have many subordinates
    }
}
