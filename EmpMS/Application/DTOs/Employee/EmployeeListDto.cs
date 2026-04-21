using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public decimal AnnualCTC { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
    }
}
