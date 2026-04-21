using Application.DTOs.Salary;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Application.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly ISalaryStructureRepository _salaryStructureRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SalaryService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SalaryService(
            ISalaryRepository salaryRepository,
            ISalaryStructureRepository salaryStructureRepository,
            IEmployeeRepository employeeRepository,
            IAttendanceRepository attendanceRepository,
            ILeaveRepository leaveRepository,
            IMapper mapper,
            ILogger<SalaryService> logger,
            IUnitOfWork unitOfWork)
        {
            _salaryRepository = salaryRepository;
            _salaryStructureRepository = salaryStructureRepository;
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
            _leaveRepository = leaveRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SalaryResponseDto>> GenerateMonthlySalaryAsync(int month, int year, string generatedBy)
        {
            if (month < 1 || month > 12)
                throw new BadRequestException("Month must be between 1 and 12!");

            if (year < 2020 || year > DateTime.Now.Year + 1)
                throw new BadRequestException("Invalid year!");

            if (year > DateTime.Now.Year || (year == DateTime.Now.Year && month > DateTime.Now.Month))
                throw new BadRequestException("Cannot generate salary for future months!");

            _logger.LogInformation("Starting salary generation for {Month}/{Year} by {User}", month, year, generatedBy);

            var employees = await _employeeRepository.GetAllAsync(1, int.MaxValue, null, null);
            employees = employees.Where(e => e.IsActive).ToList();

            if (employees.Count == 0)
                throw new NotFoundException("No active employees found!");

            var structure = await _salaryStructureRepository.GetAllActiveAsync();
            if (structure.Count == 0)
                throw new NotFoundException("Salary structure not configured! Seed the SalaryStructures table first.");

            var generatedSalaries = new List<Salary>();
            int skippedCount = 0;

            foreach (var employee in employees)
            {
                bool exists = await _salaryRepository.ExistsAsync(employee.Id, month, year);
                if (exists)
                {
                    _logger.LogWarning("Salary already exists for Employee {EmpId} for {Month}/{Year} — skipping", employee.Id, month, year);
                    skippedCount++;
                    continue;
                }

                var salary = await CalculateEmployeeSalary(employee, month, year, structure, generatedBy);
                generatedSalaries.Add(salary);
            }

            if (generatedSalaries.Count == 0)
                throw new BadRequestException($"Salary already generated for all employees for {month}/{year}!");

            // SAVE ALL AT ONCE
            await _salaryRepository.AddRangeAsync(generatedSalaries);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Salary generated for {Count} employees. Skipped {Skipped} (already exists)",
                generatedSalaries.Count, skippedCount);

            // Re-fetch with includes for proper DTO mapping
            var result = await _salaryRepository.GetAllByMonthYearAsync(month, year);
            return _mapper.Map<List<SalaryResponseDto>>(result);
        }

        public async Task<SalaryResponseDto> GetMySalaryAsync(string email, int month, int year)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var salary = await _salaryRepository.GetByEmployeeMonthYearAsync(employee.Id, month, year);
            if (salary == null)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                throw new NotFoundException($"Payslip not found for {monthName} {year}!");
            }

            return _mapper.Map<SalaryResponseDto>(salary);
        }

        public async Task<List<SalaryResponseDto>> GetEmployeeSalaryAsync(int empId, int? month, int? year)
        {
            if (empId <= 0) throw new BadRequestException("Invalid employee ID!");

            var salaries = await _salaryRepository.GetByEmployeeAsync(empId, month, year);
            if (salaries.Count == 0) throw new NotFoundException("No salary records found for this employee!");

            return _mapper.Map<List<SalaryResponseDto>>(salaries);
        }

        public async Task<List<SalaryResponseDto>> GetAllSalariesAsync(int month, int year)
        {
            var salaries = await _salaryRepository.GetAllByMonthYearAsync(month, year);
            if (salaries.Count == 0) throw new NotFoundException($"No salary records found for {month}/{year}!");

            return _mapper.Map<List<SalaryResponseDto>>(salaries);
        }

        public async Task<SalaryResponseDto> UpdateSalaryAsync(int id, SalaryUpdateDto dto, string updatedBy)
        {
            var salary = await _salaryRepository.GetByIdAsync(id);
            if (salary == null) throw new NotFoundException("Salary record not found!");

            // Apply corrections — only update fields that were provided
            if (dto.Bonus.HasValue)
                salary.Bonus = dto.Bonus.Value;

            if (dto.IncomeTax.HasValue)
                salary.IncomeTax = dto.IncomeTax.Value;

            if (dto.LopDeduction.HasValue)
                salary.LopDeduction = dto.LopDeduction.Value;

            // RECALCULATE TOTALS
            salary.GrossEarnings = salary.Basic + salary.HRA + salary.DA
                                 + salary.TravelAllowance + salary.SpecialAllowance + salary.Bonus;

            salary.TotalDeductions = salary.EmployeePF + salary.ProfessionalTax
                                   + salary.IncomeTax + salary.LopDeduction;

            salary.NetSalary = salary.GrossEarnings - salary.TotalDeductions;

            // Make net salary zero if deductions exceed gross (edge case)
            if (salary.NetSalary < 0)
                salary.NetSalary = 0;

            salary.PayslipStatus = "Corrected";
            salary.UpdatedAt = DateTime.Now;
            salary.UpdatedBy = updatedBy;

            _salaryRepository.Update(salary);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Salary {Id} corrected by {User}. New Net: {Net}", id, updatedBy, salary.NetSalary);

            // Re-fetch with includes
            var updated = await _salaryRepository.GetByIdAsync(id);
            return _mapper.Map<SalaryResponseDto>(updated);
        }

        public async Task<List<SalaryReportDto>> GetYearlySalaryReportAsync(int year)
        {
            var salaries = await _salaryRepository.GetYearlyAllAsync(year);
            if (salaries.Count == 0) throw new NotFoundException($"No salary records found for year {year}!");

            var report = salaries
                .GroupBy(s => s.Month)
                .Select(group =>
                {
                    var records = group.ToList();
                    return new SalaryReportDto
                    {
                        Month = group.Key,
                        MonthName = new DateTime(year, group.Key, 1).ToString("MMMM yyyy"),
                        TotalEmployees = records.Count,
                        TotalGrossEarnings = records.Sum(s => s.GrossEarnings),
                        TotalDeductions = records.Sum(s => s.TotalDeductions),
                        TotalNetSalary = records.Sum(s => s.NetSalary),
                        TotalEmployerPF = records.Sum(s => s.EmployerPF),
                        TotalGratuity = records.Sum(s => s.Gratuity),
                        TotalCostToCompany = records.Sum(s => s.NetSalary + s.EmployerPF + s.Gratuity),
                        AverageNetSalary = Math.Round(records.Average(s => s.NetSalary), 2)
                    };
                })
                .OrderBy(r => r.Month)
                .ToList();

            return report;
        }


        private async Task<Salary> CalculateEmployeeSalary(
            Employee employee, int month, int year,
            List<SalaryStructure> structure, string generatedBy)
        {
            decimal annualCTC = employee.AnnualCTC;        // Employee.AnnualCTC = Annual CTC
            decimal monthlyCTC = Math.Round(annualCTC / 12, 2);

            // Calculate earning components
            decimal basic = Math.Round((annualCTC * 0.40m) / 12, 2);   // 40% of CTC
            decimal hra = Math.Round(basic * 0.50m, 2);                 // 50% of Basic
            decimal da = Math.Round(basic * 0.10m, 2);                  // 10% of Basic
            decimal ta = 1600m;                                         // Fixed ₹1,600

            // Employer contributions (used to calculate Special Allowance)
            decimal employerPF = Math.Round(basic * 0.12m, 2);
            if (employerPF > 1800) employerPF = 1800;                  // Cap at ₹1,800

            decimal gratuity = Math.Round(basic * 0.0481m, 2);

            // Special Allowance = Remaining balance (CTC minus everything else)
            decimal specialAllowance = monthlyCTC - basic - hra - da - ta - employerPF - gratuity;
            if (specialAllowance < 0) specialAllowance = 0;

            decimal bonus = 0;  // Default 0, admin can add later via PUT

            // Query attendance for the month
            int totalCalendarDays = DateTime.DaysInMonth(year, month);
            int totalWorkingDays = Enumerable.Range(1, totalCalendarDays)
                .Count(day =>
                {
                    var date = new DateTime(year, month, day);
                    return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
                });

            var attendances = await _attendanceRepository.GetByEmployeeMonthlyAsync(employee.Id, month, year);

            int presentDays = attendances.Count(a => a.Status == "Present");
            int halfDays = attendances.Count(a => a.Status == "HalfDay");
            int onLeaveDays = attendances.Count(a => a.Status == "OnLeave");

            // Query leave data — paid vs unpaid
            var leaveRequests = await _leaveRepository.GetRequestsByEmployeeAsync(employee.Id);

            // Filter to approved leaves that fall within this month
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = new DateOnly(year, month, totalCalendarDays);

            var approvedLeavesThisMonth = leaveRequests
                .Where(lr => lr.Status == "Approved"
                         && lr.StartDate <= monthEnd
                         && lr.EndDate >= monthStart)
                .ToList();

            int paidLeaveDays = 0;
            int unpaidLeaveDays = 0;

            foreach (var leave in approvedLeavesThisMonth)
            {
                // Calculate how many leave days fall within THIS month
                var effectiveStart = leave.StartDate < monthStart ? monthStart : leave.StartDate;
                var effectiveEnd = leave.EndDate > monthEnd ? monthEnd : leave.EndDate;
                int daysInMonth = effectiveEnd.DayNumber - effectiveStart.DayNumber + 1;

                if (leave.LeaveType != null && leave.LeaveType.IsPaid)
                    paidLeaveDays += daysInMonth;
                else
                    unpaidLeaveDays += daysInMonth;
            }

            // Absent days = working days not accounted for
            decimal effectivePresent = presentDays + (halfDays * 0.5m);
            int absentDays = totalWorkingDays - (int)effectivePresent - paidLeaveDays - onLeaveDays;
            if (absentDays < 0) absentDays = 0;

            // Total LOP = unpaid leaves + unaccounted absent days
            int totalLopDays = unpaidLeaveDays + absentDays;

            // Calculate LOP deduction
            decimal grossEarnings = basic + hra + da + ta + specialAllowance + bonus;

            // LOP uses calendar days (industry standard — same as Keka)
            decimal perDayRate = Math.Round(grossEarnings / totalCalendarDays, 2);
            decimal lopDeduction = Math.Round(perDayRate * totalLopDays, 2);

            // Calculate statutory deductions
            // Employee PF — 12% of Basic, capped at ₹1,800
            decimal employeePF = Math.Round(basic * 0.12m, 2);
            if (employeePF > 1800) employeePF = 1800;

            // Professional Tax — ₹200/month (if gross > ₹15,000)
            decimal professionalTax = grossEarnings > 15000 ? 200 : 0;

            // Income Tax (TDS) — New Tax Regime FY 2025-26
            decimal incomeTax = CalculateMonthlyTDS(annualCTC);

            // Final calculation
            decimal totalDeductions = employeePF + professionalTax + incomeTax + lopDeduction;
            decimal netSalary = grossEarnings - totalDeductions;
            if (netSalary < 0) netSalary = 0;

            // Build and return Salary entity
            return new Salary
            {
                EmployeeId = employee.Id,
                Month = month,
                Year = year,
                AnnualCTC = annualCTC,

                // Earnings
                Basic = basic,
                HRA = hra,
                DA = da,
                TravelAllowance = ta,
                SpecialAllowance = specialAllowance,
                Bonus = bonus,

                // Employee Deductions
                EmployeePF = employeePF,
                ProfessionalTax = professionalTax,
                IncomeTax = incomeTax,

                // Employer Contributions
                EmployerPF = employerPF,
                Gratuity = gratuity,

                // Attendance
                TotalWorkingDays = totalWorkingDays,
                PresentDays = presentDays,
                PaidLeaveDays = paidLeaveDays,
                UnpaidLeaveDays = totalLopDays,
                HalfDays = halfDays,
                AbsentDays = absentDays,
                LopDeduction = lopDeduction,

                // Totals
                GrossEarnings = grossEarnings,
                TotalDeductions = totalDeductions,
                NetSalary = netSalary,

                // Metadata
                PayslipStatus = "Generated",
                GeneratedDate = DateTime.Now,
                GeneratedBy = generatedBy,
                CreatedAt = DateTime.Now,
                CreatedBy = generatedBy
            };
        }

        private decimal CalculateMonthlyTDS(decimal annualCTC)
        {
            // Standard Deduction under New Regime
            decimal standardDeduction = 75000;
            decimal taxableIncome = annualCTC - standardDeduction;

            if (taxableIncome <= 0) return 0;

            // New Tax Regime Slabs FY 2025-26
            decimal tax = 0;

            if (taxableIncome <= 400000)
            {
                tax = 0;
            }
            else if (taxableIncome <= 800000)
            {
                tax = (taxableIncome - 400000) * 0.05m;
            }
            else if (taxableIncome <= 1200000)
            {
                tax = 400000 * 0.05m
                    + (taxableIncome - 800000) * 0.10m;
            }
            else if (taxableIncome <= 1600000)
            {
                tax = 400000 * 0.05m
                    + 400000 * 0.10m
                    + (taxableIncome - 1200000) * 0.15m;
            }
            else if (taxableIncome <= 2000000)
            {
                tax = 400000 * 0.05m
                    + 400000 * 0.10m
                    + 400000 * 0.15m
                    + (taxableIncome - 1600000) * 0.20m;
            }
            else if (taxableIncome <= 2400000)
            {
                tax = 400000 * 0.05m
                    + 400000 * 0.10m
                    + 400000 * 0.15m
                    + 400000 * 0.20m
                    + (taxableIncome - 2000000) * 0.25m;
            }
            else
            {
                tax = 400000 * 0.05m
                    + 400000 * 0.10m
                    + 400000 * 0.15m
                    + 400000 * 0.20m
                    + 400000 * 0.25m
                    + (taxableIncome - 2400000) * 0.30m;
            }

            // Rebate u/s 87A: If taxable income ≤ ₹7,00,000 → tax = 0
            if (taxableIncome <= 700000)
                tax = 0;

            // Add 4% Health & Education Cess
            tax = Math.Round(tax * 1.04m, 2);

            // Monthly TDS = Annual Tax ÷ 12
            decimal monthlyTDS = Math.Round(tax / 12, 2);

            return monthlyTDS;
        }
    }
}
