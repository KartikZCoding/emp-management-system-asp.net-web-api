using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SalaryStructure
    {
        public int Id { get; set; }
        public string ComponentName { get; set; }       // "Basic Salary", "HRA", "Employee PF", etc.
        public string ComponentType { get; set; }       // "Earning", "Deduction", "EmployerContribution"
        public string CalculationType { get; set; }     // "PercentageOfCTC", "PercentageOfBasic", "Fixed", "Remaining", "TaxSlab"
        public decimal Value { get; set; }              // 40 means 40%, or 1600 means ₹1600 fixed
        public decimal? MaxLimit { get; set; }          // e.g., PF capped at ₹1,800/month — null means no cap
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }           // order on payslip (1 = Basic shown first)
        //audit fields
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
