using Application.DTOs.Leave;
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
    public class LeaveServiceTests
    {
        private readonly Mock<ILeaveRepository> _mockLeaveRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LeaveService>> _mockLogger;
        private readonly Mock<IEmailService> _mockEmail;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly LeaveService _service;

        public LeaveServiceTests()
        {
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LeaveService>>();
            _mockEmail = new Mock<IEmailService>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new LeaveService(
                _mockLeaveRepo.Object, _mockEmpRepo.Object, _mockMapper.Object,
                _mockLogger.Object, _mockEmail.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task GetAllLeaveTypes_Empty_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetAllLeaveTypesAsync()).ReturnsAsync(new List<LeaveType>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllLeaveTypesAsync());
        }

        [Fact]
        public async Task GetAllLeaveTypes_HasData_ReturnsMappedList()
        {
            var types = new List<LeaveType> { new LeaveType { Id = 1, Name = "Sick" } };
            var dtos = new List<LeaveTypeResponseDto> { new LeaveTypeResponseDto { Id = 1, Name = "Sick" } };
            _mockLeaveRepo.Setup(r => r.GetAllLeaveTypesAsync()).ReturnsAsync(types);
            _mockMapper.Setup(m => m.Map<List<LeaveTypeResponseDto>>(types)).Returns(dtos);

            var result = await _service.GetAllLeaveTypesAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task GetLeaveTypeById_NotFound_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetLeaveTypeByIdAsync(999)).ReturnsAsync((LeaveType)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLeaveTypeByIdAsync(999));
        }

        [Fact]
        public async Task CreateLeaveType_Duplicate_ThrowsBadRequest()
        {
            _mockLeaveRepo.Setup(r => r.LeaveTypeExistsAsync("Sick")).ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateLeaveTypeAsync(new LeaveTypeDto { Name = "Sick" }));
        }

        [Fact]
        public async Task CreateLeaveType_Valid_CreatesAndSaves()
        {
            _mockLeaveRepo.Setup(r => r.LeaveTypeExistsAsync("New")).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<LeaveTypeResponseDto>(It.IsAny<LeaveType>())).Returns(new LeaveTypeResponseDto());

            await _service.CreateLeaveTypeAsync(new LeaveTypeDto { Name = "New", DefaultDays = 10 });

            _mockLeaveRepo.Verify(r => r.CreateLeaveTypeAsync(It.IsAny<LeaveType>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateLeaveType_NotFound_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetLeaveTypeByIdAsync(999)).ReturnsAsync((LeaveType)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateLeaveTypeAsync(999, new LeaveTypeDto()));
        }

        [Fact]
        public async Task DeleteLeaveType_NotFound_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetLeaveTypeByIdAsync(999)).ReturnsAsync((LeaveType)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteLeaveTypeAsync(999));
        }

        [Fact]
        public async Task DeleteLeaveType_Valid_SoftDeletesAndSaves()
        {
            var lt = new LeaveType { Id = 1, IsActive = true };
            _mockLeaveRepo.Setup(r => r.GetLeaveTypeByIdAsync(1)).ReturnsAsync(lt);

            await _service.DeleteLeaveTypeAsync(1);

            Assert.False(lt.IsActive);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMyBalances_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyBalancesAsync("bad@t.com", 2026));
        }

        [Fact]
        public async Task GetMyBalances_NoBalances_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockLeaveRepo.Setup(r => r.GetBalancesByEmployeeAsync(1, 2026)).ReturnsAsync(new List<LeaveBalance>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyBalancesAsync("t@t.com", 2026));
        }

        [Fact]
        public async Task AssignBalances_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.AssignBalancesAsync(999, 2026));
        }

        [Fact]
        public async Task AssignBalances_AlreadyAssigned_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1 });
            _mockLeaveRepo.Setup(r => r.GetBalancesByEmployeeAsync(1, 2026)).ReturnsAsync(new List<LeaveBalance> { new LeaveBalance() });
            await Assert.ThrowsAsync<BadRequestException>(() => _service.AssignBalancesAsync(1, 2026));
        }

        [Fact]
        public async Task ApplyLeave_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ApplyLeaveAsync("bad@t.com", new LeaveRequestDto()));
        }

        [Fact]
        public async Task ApplyLeave_PastDate_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var dto = new LeaveRequestDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                LeaveTypeId = 1
            };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.ApplyLeaveAsync("t@t.com", dto));
        }

        [Fact]
        public async Task ApplyLeave_EndBeforeStart_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var future = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
            var dto = new LeaveRequestDto
            {
                StartDate = future,
                EndDate = future.AddDays(-2),
                LeaveTypeId = 1
            };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.ApplyLeaveAsync("t@t.com", dto));
        }

        [Fact]
        public async Task GetMyRequests_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyRequestsAsync("bad@t.com"));
        }

        [Fact]
        public async Task GetPendingRequests_None_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetPendingRequestAsync()).ReturnsAsync(new List<LeaveRequest>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPendingRequestAsync());
        }

        [Fact]
        public async Task ApproveLeave_NotFound_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(999)).ReturnsAsync((LeaveRequest)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveLeaveAsync(999, 1, null));
        }

        [Fact]
        public async Task ApproveLeave_AlreadyApproved_ThrowsBadRequest()
        {
            var request = new LeaveRequest { Id = 1, Status = "Approved" };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.ApproveLeaveAsync(1, 1, null));
        }

        [Fact]
        public async Task RejectLeave_NotFound_ThrowsNotFound()
        {
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(999)).ReturnsAsync((LeaveRequest)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.RejectLeaveAsync(999, 1, null));
        }

        [Fact]
        public async Task RejectLeave_AlreadyRejected_ThrowsBadRequest()
        {
            var request = new LeaveRequest { Id = 1, Status = "Rejected" };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.RejectLeaveAsync(1, 1, null));
        }

        [Fact]
        public async Task CancelLeave_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelLeaveAsync("bad@t.com", 1));
        }

        [Fact]
        public async Task CancelLeave_RequestNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(999)).ReturnsAsync((LeaveRequest)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelLeaveAsync("t@t.com", 999));
        }

        [Fact]
        public async Task CancelLeave_NotOwnRequest_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var request = new LeaveRequest { Id = 1, EmployeeId = 99, Status = "Pending" };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CancelLeaveAsync("t@t.com", 1));
        }

        [Fact]
        public async Task CancelLeave_AlreadyRejected_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var request = new LeaveRequest { Id = 1, EmployeeId = 1, Status = "Rejected" };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CancelLeaveAsync("t@t.com", 1));
        }

        [Fact]
        public async Task CancelLeave_AlreadyCancelled_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var request = new LeaveRequest { Id = 1, EmployeeId = 1, Status = "Cancelled" };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CancelLeaveAsync("t@t.com", 1));
        }

        // Approve Deducts Balance

        [Fact]
        public async Task ApproveLeave_Valid_DeductsBalanceAndSendsEmail()
        {
            var request = new LeaveRequest
            {
                Id = 1, EmployeeId = 1, LeaveTypeId = 1, TotalDays = 3,
                Status = "Pending", StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 3),
                Employee = new Employee { FirstName = "Kartik", LastName = "Z", Email = "k@t.com" },
                LeaveType = new LeaveType { Name = "Casual" }
            };
            var balance = new LeaveBalance { RemainingLeaves = 10, UsedLeaves = 2 };

            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockLeaveRepo.Setup(r => r.GetBalanceAsync(1, 1, 2026)).ReturnsAsync(balance);
            _mockMapper.Setup(m => m.Map<LeaveRequestResponseDto>(request)).Returns(new LeaveRequestResponseDto());

            await _service.ApproveLeaveAsync(1, 5, "Enjoy your leave");

            Assert.Equal("Approved", request.Status);
            Assert.Equal(5, request.ApprovedById);
            Assert.Equal(5, balance.UsedLeaves);
            Assert.Equal(7, balance.RemainingLeaves);
            _mockEmail.Verify(e => e.SendEmailAsync("k@t.com", "Leave Approved", It.Is<string>(b => b.Contains("APPROVED"))), Times.Once);
        }

        // Approve Insufficient Balance

        [Fact]
        public async Task ApproveLeave_InsufficientBalance_ThrowsBadRequest()
        {
            var request = new LeaveRequest
            {
                Id = 1, EmployeeId = 1, LeaveTypeId = 1, TotalDays = 5,
                Status = "Pending", StartDate = new DateOnly(2026, 5, 1)
            };
            var balance = new LeaveBalance { RemainingLeaves = 2 };

            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockLeaveRepo.Setup(r => r.GetBalanceAsync(1, 1, 2026)).ReturnsAsync(balance);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.ApproveLeaveAsync(1, 5, null));
        }

        // Reject Sends Email

        [Fact]
        public async Task RejectLeave_Valid_SetsStatusAndSendsEmail()
        {
            var request = new LeaveRequest
            {
                Id = 1, EmployeeId = 1, Status = "Pending",
                StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 3), TotalDays = 3,
                Employee = new Employee { FirstName = "John", LastName = "Doe", Email = "j@t.com" },
                LeaveType = new LeaveType { Name = "Sick" }
            };
            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockMapper.Setup(m => m.Map<LeaveRequestResponseDto>(request)).Returns(new LeaveRequestResponseDto());

            await _service.RejectLeaveAsync(1, 5, "Team needs you");

            Assert.Equal("Rejected", request.Status);
            Assert.Equal(5, request.ApprovedById);
            Assert.Equal("Team needs you", request.DecisionNote);
            _mockEmail.Verify(e => e.SendEmailAsync("j@t.com", "Leave Rejected", It.Is<string>(b => b.Contains("REJECTED"))), Times.Once);
        }

        // Cancel Approved Restores Balance

        [Fact]
        public async Task CancelLeave_ApprovedRequest_RestoresBalance()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var request = new LeaveRequest
            {
                Id = 1, EmployeeId = 1, LeaveTypeId = 1, TotalDays = 3,
                Status = "Approved", StartDate = new DateOnly(2026, 5, 1)
            };
            var balance = new LeaveBalance { UsedLeaves = 5, RemainingLeaves = 7 };

            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockLeaveRepo.Setup(r => r.GetBalanceAsync(1, 1, 2026)).ReturnsAsync(balance);
            _mockMapper.Setup(m => m.Map<LeaveRequestResponseDto>(request)).Returns(new LeaveRequestResponseDto());

            await _service.CancelLeaveAsync("t@t.com", 1);

            Assert.Equal("Cancelled", request.Status);
            Assert.Equal(2, balance.UsedLeaves);
            Assert.Equal(10, balance.RemainingLeaves);
        }

        // Cancel Pending Does NOT Touch Balance

        [Fact]
        public async Task CancelLeave_PendingRequest_DoesNotTouchBalance()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var request = new LeaveRequest { Id = 1, EmployeeId = 1, Status = "Pending" };

            _mockLeaveRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockMapper.Setup(m => m.Map<LeaveRequestResponseDto>(request)).Returns(new LeaveRequestResponseDto());

            await _service.CancelLeaveAsync("t@t.com", 1);

            Assert.Equal("Cancelled", request.Status);
            _mockLeaveRepo.Verify(r => r.GetBalanceAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        // CreateLeaveType Sets Audit Fields

        [Fact]
        public async Task CreateLeaveType_SetsIsActiveAndCreatedAt()
        {
            _mockLeaveRepo.Setup(r => r.LeaveTypeExistsAsync("New")).ReturnsAsync(false);

            LeaveType captured = null;
            _mockLeaveRepo.Setup(r => r.CreateLeaveTypeAsync(It.IsAny<LeaveType>()))
                .Callback<LeaveType>(lt => captured = lt)
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<LeaveTypeResponseDto>(It.IsAny<LeaveType>())).Returns(new LeaveTypeResponseDto());

            await _service.CreateLeaveTypeAsync(new LeaveTypeDto { Name = "New", DefaultDays = 10, IsPaid = true });

            Assert.NotNull(captured);
            Assert.True(captured.IsActive);
            Assert.True((DateTime.Now - captured.CreatedAt).TotalSeconds < 5);
            Assert.True(captured.IsPaid);
        }
    }
}


