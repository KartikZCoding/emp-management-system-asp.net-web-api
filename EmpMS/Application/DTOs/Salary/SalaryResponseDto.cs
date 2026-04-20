namespace Application.DTOs.Salary
{
    public class SalaryResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public decimal AnnualCTC { get; set; }

        // Earnings breakdown
        public decimal Basic { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal TravelAllowance { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal GrossEarnings { get; set; }

        // Deductions breakdown
        public decimal EmployeePF { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal LopDeduction { get; set; }
        public decimal TotalDeductions { get; set; }

        // Employer contributions
        public decimal EmployerPF { get; set; }
        public decimal Gratuity { get; set; }

        // Attendance summary
        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int PaidLeaveDays { get; set; }
        public int UnpaidLeaveDays { get; set; }
        public int HalfDays { get; set; }
        public int AbsentDays { get; set; }

        // Final
        public decimal NetSalary { get; set; }
        public string PayslipStatus { get; set; }
        public DateTime GeneratedDate { get; set; }
    }
}
