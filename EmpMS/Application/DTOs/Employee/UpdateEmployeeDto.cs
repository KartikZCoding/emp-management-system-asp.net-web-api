using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public int DesignationId { get; set; }
        public int? ManagerId { get; set; }
        [Required]
        public decimal? AnnualCTC { get; set; }
    }
}
