using Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPerformanceReviewService
    {
        Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId, string createdBy);
        Task<List<ReviewResponseDto>> GetEmployeeReviewsAsync(int employeeId);
        Task<List<ReviewResponseDto>> GetMyReviewsAsync(string email);
        Task<ReviewResponseDto> UpdateReviewAsync(int id, UpdateReviewDto dto, string updatedBy);
        Task DeleteReviewAsync(int id);
        Task<DepartmentReviewSummaryDto> GetDepartmentReviewSummaryAsync(int departmentId, int year);
    }
}
