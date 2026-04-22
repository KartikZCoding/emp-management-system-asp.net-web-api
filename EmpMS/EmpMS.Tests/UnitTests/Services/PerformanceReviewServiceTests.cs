using Application.DTOs.Review;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class PerformanceReviewServiceTests
    {
        private readonly Mock<IPerformanceReviewRepository> _mockReviewRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IDepartmentRepository> _mockDeptRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PerformanceReviewService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly PerformanceReviewService _service;

        public PerformanceReviewServiceTests()
        {
            _mockReviewRepo = new Mock<IPerformanceReviewRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockDeptRepo = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PerformanceReviewService>>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new PerformanceReviewService(
                _mockReviewRepo.Object, _mockEmpRepo.Object, _mockDeptRepo.Object,
                _mockMapper.Object, _mockLogger.Object, _mockUoW.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public async Task CreateReview_InvalidRating_ThrowsBadRequest(int rating)
        {
            var dto = new CreateReviewDto { EmployeeId = 1, Rating = rating, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewAsync(dto, 2, "admin"));
        }

        [Fact]
        public async Task CreateReview_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);
            var dto = new CreateReviewDto { EmployeeId = 999, Rating = 3, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReviewAsync(dto, 2, "admin"));
        }

        [Fact]
        public async Task CreateReview_InactiveEmployee_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = false });
            var dto = new CreateReviewDto { EmployeeId = 1, Rating = 3, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReviewAsync(dto, 2, "admin"));
        }

        [Fact]
        public async Task CreateReview_ReviewerNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            _mockEmpRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);
            var dto = new CreateReviewDto { EmployeeId = 1, Rating = 3, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReviewAsync(dto, 999, "admin"));
        }

        [Fact]
        public async Task CreateReview_SelfReview_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            var dto = new CreateReviewDto { EmployeeId = 1, Rating = 3, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewAsync(dto, 1, "admin"));
        }

        [Fact]
        public async Task CreateReview_DuplicatePeriod_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            _mockEmpRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { Id = 2, IsActive = true });
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, "Q1")).ReturnsAsync(true);

            var dto = new CreateReviewDto { EmployeeId = 1, Rating = 3, ReviewPeriod = "Q1" };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewAsync(dto, 2, "admin"));
        }

        [Fact]
        public async Task CreateReview_EmptyPeriod_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            _mockEmpRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { Id = 2, IsActive = true });
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, "")).ReturnsAsync(false);

            var dto = new CreateReviewDto { EmployeeId = 1, Rating = 3, ReviewPeriod = "" };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewAsync(dto, 2, "admin"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetEmployeeReviews_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() => _service.GetEmployeeReviewsAsync(id));
        }

        [Fact]
        public async Task GetEmployeeReviews_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetEmployeeReviewsAsync(999));
        }

        [Fact]
        public async Task GetMyReviews_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyReviewsAsync("bad@t.com"));
        }

        [Fact]
        public async Task UpdateReview_NotFound_ThrowsNotFound()
        {
            _mockReviewRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PerformanceReview)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateReviewAsync(999, new UpdateReviewDto(), "admin"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task UpdateReview_InvalidRating_ThrowsBadRequest(int rating)
        {
            var review = new PerformanceReview { Id = 1, EmployeeId = 1, ReviewerId = 2 };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateReviewAsync(1, new UpdateReviewDto { Rating = rating }, "admin"));
        }

        [Fact]
        public async Task DeleteReview_NotFound_ThrowsNotFound()
        {
            _mockReviewRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PerformanceReview)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteReviewAsync(999));
        }

        [Fact]
        public async Task DeleteReview_Valid_DeletesAndSaves()
        {
            var review = new PerformanceReview { Id = 1, EmployeeId = 1, ReviewPeriod = "Q1" };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            await _service.DeleteReviewAsync(1);

            _mockReviewRepo.Verify(r => r.Delete(review), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDeptSummary_DeptNotFound_ThrowsNotFound()
        {
            _mockDeptRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetDepartmentReviewSummaryAsync(999, 2025));
        }

        // Create Happy Path with State

        [Fact]
        public async Task CreateReview_Valid_SetsAllFieldsCorrectly()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            _mockEmpRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { Id = 2, IsActive = true });
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, "Q1-2026")).ReturnsAsync(false);

            PerformanceReview captured = null;
            _mockReviewRepo.Setup(r => r.AddAsync(It.IsAny<PerformanceReview>()))
                .Callback<PerformanceReview>(pr => captured = pr)
                .Returns(Task.CompletedTask);

            var savedReview = new PerformanceReview
            {
                Id = 1, EmployeeId = 1, ReviewerId = 2, Rating = 4,
                ReviewPeriod = "Q1-2026", Comments = "Great work", Goals = "Improve code quality",
                CreatedBy = "admin", CreatedAt = DateTime.Now,
                Employee = new Employee { Id = 1, FirstName = "Kartik", LastName = "Z", Department = new Department { DepartmentName = "IT" }, Designation = new Designation { DesignationName = "Dev" } },
                Reviewer = new Employee { Id = 2, FirstName = "Manager", LastName = "One" }
            };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(savedReview);

            var dto = new CreateReviewDto
            {
                EmployeeId = 1, Rating = 4, ReviewPeriod = "Q1-2026",
                Comments = "Great work", Goals = "Improve code quality"
            };

            var result = await _service.CreateReviewAsync(dto, 2, "admin");

            Assert.NotNull(captured);
            Assert.Equal(1, captured.EmployeeId);
            Assert.Equal(2, captured.ReviewerId);
            Assert.Equal(4, captured.Rating);
            Assert.Equal("Q1-2026", captured.ReviewPeriod);
            Assert.Equal("admin", captured.CreatedBy);
            Assert.True((DateTime.Now - captured.CreatedAt).TotalSeconds < 5);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // Update Partial Fields Only

        [Fact]
        public async Task UpdateReview_OnlyRating_PreservesOtherFields()
        {
            var review = new PerformanceReview
            {
                Id = 1, EmployeeId = 1, ReviewerId = 2, Rating = 3,
                Comments = "Original", Goals = "Original goals",
                ReviewPeriod = "Q1-2026"
            };

            var refetched = new PerformanceReview
            {
                Id = 1, EmployeeId = 1, ReviewerId = 2, Rating = 5,
                Comments = "Original", Goals = "Original goals",
                ReviewPeriod = "Q1-2026",
                Employee = new Employee { Id = 1, FirstName = "A", LastName = "B", Department = new Department { DepartmentName = "IT" }, Designation = new Designation { DesignationName = "Dev" } },
                Reviewer = new Employee { Id = 2, FirstName = "C", LastName = "D" }
            };

            var callCount = 0;
            _mockReviewRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? review : refetched;
            });

            await _service.UpdateReviewAsync(1, new UpdateReviewDto { Rating = 5 }, "admin");

            Assert.Equal(5, review.Rating);
            Assert.Equal("Original", review.Comments);
            Assert.Equal("Original goals", review.Goals);
        }

        // Delete Does Not Call SaveChanges
        // if Repository.Delete throws

        [Fact]
        public async Task DeleteReview_Valid_OperationOrder()
        {
            var callOrder = new List<string>();
            var review = new PerformanceReview { Id = 1 };

            _mockReviewRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            _mockReviewRepo.Setup(r => r.Delete(review)).Callback(() => callOrder.Add("Delete"));
            _mockUoW.Setup(u => u.SaveChangesAsync()).Callback(() => callOrder.Add("Save")).ReturnsAsync(1);

            await _service.DeleteReviewAsync(1);

            Assert.Equal(new[] { "Delete", "Save" }, callOrder);
        }

        // Boundary Rating Values

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task CreateReview_BoundaryRatings_Accepted(int rating)
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, IsActive = true });
            _mockEmpRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { Id = 2, IsActive = true });
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, "Q1")).ReturnsAsync(false);

            var savedReview = new PerformanceReview
            {
                Id = 1, EmployeeId = 1, ReviewerId = 2, Rating = rating, ReviewPeriod = "Q1",
                Employee = new Employee { Id = 1, FirstName = "A", LastName = "B", Department = new Department { DepartmentName = "IT" }, Designation = new Designation { DesignationName = "Dev" } },
                Reviewer = new Employee { Id = 2, FirstName = "C", LastName = "D" }
            };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(savedReview);

            var dto = new CreateReviewDto { EmployeeId = 1, Rating = rating, ReviewPeriod = "Q1" };

            var result = await _service.CreateReviewAsync(dto, 2, "admin");

            Assert.NotNull(result);
            Assert.Equal(rating, result.Rating);
        }
    }
}


