using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public decimal AnnualCTC { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
