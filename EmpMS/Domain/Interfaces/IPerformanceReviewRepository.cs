using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IPerformanceReviewRepository
    {
        Task<PerformanceReview?> GetByIdAsync(int id);
        Task<List<PerformanceReview>> GetByEmployeeIdAsync(int employeeId);
        Task<List<PerformanceReview>> GetByDepartmentAndYearAsync(int departmentId, int year);
        Task<bool> ExistsAsync(int employeeId, string reviewPeriod);
        Task AddAsync(PerformanceReview review);
        void Update(PerformanceReview review);
        void Delete(PerformanceReview review);
    }
}
