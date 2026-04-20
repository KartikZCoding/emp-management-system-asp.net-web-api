using Application.DTOs.Review;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private readonly IPerformanceReviewRepository _reviewRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PerformanceReviewService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PerformanceReviewService(
            IPerformanceReviewRepository reviewRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            ILogger<PerformanceReviewService> logger,
            IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId, string createdBy)
        {
            // Validate rating range
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new BadRequestException("Rating must be between 1 and 5!");

            // Validate employee exists and is active
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null || !employee.IsActive)
                throw new NotFoundException($"Active employee with ID {dto.EmployeeId} not found!");

            // Validate reviewer exists
            var reviewer = await _employeeRepository.GetByIdAsync(reviewerId);
            if (reviewer == null)
                throw new NotFoundException($"Reviewer employee with ID {reviewerId} not found!");

            // Prevent self-review
            if (dto.EmployeeId == reviewerId)
                throw new BadRequestException("A reviewer cannot review themselves!");

            // Check for duplicate review (same employee + same period)
            bool exists = await _reviewRepository.ExistsAsync(dto.EmployeeId, dto.ReviewPeriod);
            if (exists)
                throw new BadRequestException($"A review for employee {dto.EmployeeId} for period '{dto.ReviewPeriod}' already exists!");

            // Validate review period format
            if (string.IsNullOrWhiteSpace(dto.ReviewPeriod))
                throw new BadRequestException("Review period is required! (e.g., 'Q1-2026', 'Annual-2025')");

            var review = new PerformanceReview
            {
                EmployeeId = dto.EmployeeId,
                ReviewerId = reviewerId,
                ReviewPeriod = dto.ReviewPeriod.Trim(),
                Rating = dto.Rating,
                Strengths = dto.Strengths,
                Weaknesses = dto.Weaknesses,
                Comments = dto.Comments,
                Goals = dto.Goals,
                ReviewDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _reviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Performance review created for Employee {EmpId} by Reviewer {ReviewerId} for period {Period}. Rating: {Rating}/5",
                dto.EmployeeId, reviewerId, dto.ReviewPeriod, dto.Rating);

            // Re-fetch with includes for proper DTO mapping
            var saved = await _reviewRepository.GetByIdAsync(review.Id);
            return MapToResponseDto(saved!);
        }

        public async Task<List<ReviewResponseDto>> GetEmployeeReviewsAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new BadRequestException("Invalid employee ID!");

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException($"Employee with ID {employeeId} not found!");

            var reviews = await _reviewRepository.GetByEmployeeIdAsync(employeeId);

            return reviews.Select(MapToResponseDto).ToList();
        }

        public async Task<List<ReviewResponseDto>> GetMyReviewsAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null)
                throw new NotFoundException("Employee profile not found!");

            var reviews = await _reviewRepository.GetByEmployeeIdAsync(employee.Id);

            return reviews.Select(MapToResponseDto).ToList();
        }

        public async Task<ReviewResponseDto> UpdateReviewAsync(int id, UpdateReviewDto dto, string updatedBy)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new NotFoundException($"Performance review with ID {id} not found!");

            // Validate rating if provided
            if (dto.Rating.HasValue && (dto.Rating.Value < 1 || dto.Rating.Value > 5))
                throw new BadRequestException("Rating must be between 1 and 5!");

            // Apply updates — only update fields that were provided
            if (dto.Rating.HasValue)
                review.Rating = dto.Rating.Value;

            if (dto.Strengths != null)
                review.Strengths = dto.Strengths;

            if (dto.Weaknesses != null)
                review.Weaknesses = dto.Weaknesses;

            if (dto.Comments != null)
                review.Comments = dto.Comments;

            if (dto.Goals != null)
                review.Goals = dto.Goals;

            review.UpdatedAt = DateTime.Now;
            review.UpdatedBy = updatedBy;

            _reviewRepository.Update(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Performance review {Id} updated by {User}", id, updatedBy);

            // Re-fetch with includes
            var updated = await _reviewRepository.GetByIdAsync(id);
            return MapToResponseDto(updated!);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new NotFoundException($"Performance review with ID {id} not found!");

            _reviewRepository.Delete(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Performance review {Id} deleted (Employee: {EmpId}, Period: {Period})",
                id, review.EmployeeId, review.ReviewPeriod);
        }

        public async Task<DepartmentReviewSummaryDto> GetDepartmentReviewSummaryAsync(int departmentId, int year)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
                throw new NotFoundException($"Department with ID {departmentId} not found!");

            // Get all employees in the department
            var employees = await _employeeRepository.GetByDepartmentAsync(departmentId);
            var activeEmployees = employees.Where(e => e.IsActive).ToList();

            // Get all reviews for this department in the given year
            var reviews = await _reviewRepository.GetByDepartmentAndYearAsync(departmentId, year);

            // Get unique reviewed employee IDs
            var reviewedEmployeeIds = reviews.Select(r => r.EmployeeId).Distinct().ToHashSet();
            int notReviewed = activeEmployees.Count(e => !reviewedEmployeeIds.Contains(e.Id));

            var summary = new DepartmentReviewSummaryDto
            {
                DepartmentId = departmentId,
                DepartmentName = department.DepartmentName,
                Year = year,
                TotalEmployees = activeEmployees.Count,
                TotalReviewsConducted = reviews.Count,
                EmployeesNotReviewed = notReviewed,
                AverageRating = reviews.Count > 0
                    ? Math.Round((decimal)reviews.Average(r => r.Rating), 2)
                    : 0,
                RatingDistribution = new RatingDistributionDto
                {
                    Outstanding = reviews.Count(r => r.Rating == 5),
                    ExceedsExpectations = reviews.Count(r => r.Rating == 4),
                    MeetsExpectations = reviews.Count(r => r.Rating == 3),
                    NeedsImprovement = reviews.Count(r => r.Rating == 2),
                    Unsatisfactory = reviews.Count(r => r.Rating == 1)
                }
            };

            _logger.LogInformation(
                "Department review summary generated for {DeptName} ({Year}): {ReviewCount} reviews, avg rating {Avg}",
                department.DepartmentName, year, reviews.Count, summary.AverageRating);

            return summary;
        }

        // ─── Helper: Manual mapping with computed RatingLabel ─────────────────
        private ReviewResponseDto MapToResponseDto(PerformanceReview review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                EmployeeId = review.EmployeeId,
                EmployeeName = review.Employee.FirstName + " " + review.Employee.LastName,
                DepartmentName = review.Employee.Department?.DepartmentName ?? "",
                DesignationName = review.Employee.Designation?.DesignationName ?? "",
                ReviewerId = review.ReviewerId,
                ReviewerName = review.Reviewer.FirstName + " " + review.Reviewer.LastName,
                ReviewPeriod = review.ReviewPeriod,
                Rating = review.Rating,
                RatingLabel = GetRatingLabel(review.Rating),
                Strengths = review.Strengths,
                Weaknesses = review.Weaknesses,
                Comments = review.Comments,
                Goals = review.Goals,
                ReviewDate = review.ReviewDate,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        private static string GetRatingLabel(int rating) => rating switch
        {
            1 => "Unsatisfactory",
            2 => "Needs Improvement",
            3 => "Meets Expectations",
            4 => "Exceeds Expectations",
            5 => "Outstanding",
            _ => "Unknown"
        };
    }
}
