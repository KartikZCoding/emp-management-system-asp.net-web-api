namespace Application.DTOs.Salary
{
    public class SalaryReportDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalGrossEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalEmployerPF { get; set; }
        public decimal TotalGratuity { get; set; }
        public decimal TotalCostToCompany { get; set; }
        public decimal AverageNetSalary { get; set; }
    }
}
