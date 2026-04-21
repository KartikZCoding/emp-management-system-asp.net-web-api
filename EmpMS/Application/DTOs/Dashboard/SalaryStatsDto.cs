using System.Collections.Generic;

namespace Application.DTOs.Dashboard
{
    public class SalaryStatsDto
    {
        public int Year { get; set; }
        public decimal TotalAnnualExpenditure { get; set; }
        public decimal AverageMonthlySalary { get; set; }
        public decimal HighestSalary { get; set; }
        public decimal LowestSalary { get; set; }
        public List<DepartmentSalaryDto> DepartmentWiseSalary { get; set; } = new();
    }

    public class DepartmentSalaryDto
    {
        public string DepartmentName { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AvgSalary { get; set; }
    }
}
