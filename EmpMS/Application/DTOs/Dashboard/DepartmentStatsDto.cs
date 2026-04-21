using System.Collections.Generic;

namespace Application.DTOs.Dashboard
{
    public class DepartmentStatsDto
    {
        public List<DepartmentEmployeeCountDto> Departments { get; set; } = new();
        public int TotalActiveEmployees { get; set; }
    }

    public class DepartmentEmployeeCountDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
        public double Percentage { get; set; }
    }
}
