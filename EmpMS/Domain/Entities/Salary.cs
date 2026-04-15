using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Salary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Month { get; set; }                  // 1 to 12
        public int Year { get; set; }                   // e.g., 2026

        // Annual reference — snapshot of CTC at the time salary was generated
        // (if employee gets a raise mid-year, old payslips still show old CTC)
        public decimal AnnualCTC { get; set; }

        // Earnings (₹ amounts — already calculated)
        public decimal Basic { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal TravelAllowance { get; set; }
        public decimal SpecialAllowance { get; set; }   // balancing figure
        public decimal Bonus { get; set; }

        // Employee Deductions
        public decimal EmployeePF { get; set; }         // 12% of Basic, max ₹1,800
        public decimal ProfessionalTax { get; set; }    // ₹200/month
        public decimal IncomeTax { get; set; }          // TDS — monthly

        // Employer Contributions (shown on payslip, NOT deducted from salary)
        public decimal EmployerPF { get; set; }         // 12% of Basic, max ₹1,800
        public decimal Gratuity { get; set; }           // 4.81% of Basic

        // Attendance-based fields
        public int TotalWorkingDays { get; set; }       // weekdays in the month (Mon-Fri)
        public int PresentDays { get; set; }            // status = "Present"
        public int PaidLeaveDays { get; set; }          // approved leaves where LeaveType.IsPaid = true
        public int UnpaidLeaveDays { get; set; }        // LOP days
        public int HalfDays { get; set; }               // status = "HalfDay"
        public int AbsentDays { get; set; }             // days with no attendance & no leave
        public decimal LopDeduction { get; set; }       // (Gross / CalendarDays) × LOP days

        // Totals
        public decimal GrossEarnings { get; set; }      // Basic + HRA + DA + TA + SpecialAllow + Bonus
        public decimal TotalDeductions { get; set; }    // EmployeePF + PT + TDS + LOP
        public decimal NetSalary { get; set; }          // GrossEarnings - TotalDeductions

        // Metadata
        public string PayslipStatus { get; set; }       // "Generated", "Corrected", "OnHold"
        public DateTime GeneratedDate { get; set; }
        public string? GeneratedBy { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation property
        public Employee Employee { get; set; }
    }
}
