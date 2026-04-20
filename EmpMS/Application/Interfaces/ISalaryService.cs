using Application.DTOs.Salary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISalaryService
    {
        Task<List<SalaryResponseDto>> GenerateMonthlySalaryAsync(int month, int year, string generatedBy);
        Task<SalaryResponseDto> GetMySalaryAsync(string email, int month, int year);
        Task<List<SalaryResponseDto>> GetEmployeeSalaryAsync(int empId, int? month, int? year);
        Task<List<SalaryResponseDto>> GetAllSalariesAsync(int month, int year);
        Task<SalaryResponseDto> UpdateSalaryAsync(int id, SalaryUpdateDto dto, string updatedBy);
        Task<List<SalaryReportDto>> GetYearlySalaryReportAsync(int year);
    }
}
