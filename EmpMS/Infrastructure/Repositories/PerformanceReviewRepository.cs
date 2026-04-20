using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class PerformanceReviewRepository : IPerformanceReviewRepository
    {
        private readonly AppDbContext _appDbContext;

        public PerformanceReviewRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PerformanceReview?> GetByIdAsync(int id)
        {
            return await _appDbContext.PerformanceReviews
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Department)
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(pr => pr.Reviewer)
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<List<PerformanceReview>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _appDbContext.PerformanceReviews
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Department)
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(pr => pr.Reviewer)
                .Where(pr => pr.EmployeeId == employeeId)
                .OrderByDescending(pr => pr.ReviewDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PerformanceReview>> GetByDepartmentAndYearAsync(int departmentId, int year)
        {
            return await _appDbContext.PerformanceReviews
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Department)
                .Include(pr => pr.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(pr => pr.Reviewer)
                .Where(pr => pr.Employee.DepartmentId == departmentId
                          && pr.ReviewDate.Year == year)
                .OrderBy(pr => pr.Employee.FirstName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int employeeId, string reviewPeriod)
        {
            return await _appDbContext.PerformanceReviews
                .AnyAsync(pr => pr.EmployeeId == employeeId
                             && pr.ReviewPeriod == reviewPeriod);
        }

        public async Task AddAsync(PerformanceReview review)
        {
            await _appDbContext.PerformanceReviews.AddAsync(review);
        }

        public void Update(PerformanceReview review)
        {
            _appDbContext.PerformanceReviews.Update(review);
        }

        public void Delete(PerformanceReview review)
        {
            _appDbContext.PerformanceReviews.Remove(review);
        }
    }
}
