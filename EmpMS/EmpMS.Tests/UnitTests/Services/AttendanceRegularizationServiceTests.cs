using Application.DTOs.Attendance;
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
    public class AttendanceRegularizationServiceTests
    {
        private readonly Mock<IAttendanceRegularizationRepository> _mockRegRepo;
        private readonly Mock<IAttendanceRepository> _mockAttRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AttendanceRegularizationService>> _mockLogger;
        private readonly Mock<IEmailService> _mockEmail;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly AttendanceRegularizationService _service;

        public AttendanceRegularizationServiceTests()
        {
            _mockRegRepo = new Mock<IAttendanceRegularizationRepository>();
            _mockAttRepo = new Mock<IAttendanceRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AttendanceRegularizationService>>();
            _mockEmail = new Mock<IEmailService>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new AttendanceRegularizationService(
                _mockRegRepo.Object, _mockAttRepo.Object, _mockEmpRepo.Object,
                _mockMapper.Object, _mockLogger.Object, _mockEmail.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task CreateRequest_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CreateRequestAsync("bad@t.com", new AttendanceRegularizationRequestDto()));
        }

        [Fact]
        public async Task CreateRequest_FutureDate_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var dto = new AttendanceRegularizationRequestDto
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                RequestedCheckOut = new TimeOnly(18, 0)
            };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateRequestAsync("t@t.com", dto));
        }

        [Fact]
        public async Task CreateRequest_NoAttendance_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2));
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, pastDate)).ReturnsAsync((Attendance)null);

            var dto = new AttendanceRegularizationRequestDto { Date = pastDate, RequestedCheckOut = new TimeOnly(18, 0) };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateRequestAsync("t@t.com", dto));
        }

        [Fact]
        public async Task CreateRequest_NoMissingCheckout_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2));
            var att = new Attendance
            {
                Id = 1,
                AttendanceLogs = new List<AttendanceLog>
                {
                    new AttendanceLog { CheckIn = DateTime.Now.AddHours(-8), CheckOut = DateTime.Now }
                }
            };
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, pastDate)).ReturnsAsync(att);

            var dto = new AttendanceRegularizationRequestDto { Date = pastDate, RequestedCheckOut = new TimeOnly(18, 0) };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateRequestAsync("t@t.com", dto));
        }

        [Fact]
        public async Task CreateRequest_DuplicatePending_ThrowsBadRequest()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2));
            var att = new Attendance
            {
                Id = 1,
                AttendanceLogs = new List<AttendanceLog>
                {
                    new AttendanceLog { CheckIn = DateTime.Now.AddHours(-8), CheckOut = null }
                }
            };
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, pastDate)).ReturnsAsync(att);
            _mockRegRepo.Setup(r => r.HasPendingRequestAsync(1)).ReturnsAsync(true);

            var dto = new AttendanceRegularizationRequestDto { Date = pastDate, RequestedCheckOut = new TimeOnly(18, 0) };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateRequestAsync("t@t.com", dto));
        }

        [Fact]
        public async Task GetMyRequests_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyRequestsAsync("bad@t.com"));
        }

        [Fact]
        public async Task GetMyRequests_NoRequests_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockRegRepo.Setup(r => r.GetByEmployeeIdAsync(1)).ReturnsAsync(new List<AttendanceRegularization>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMyRequestsAsync("t@t.com"));
        }

        [Fact]
        public async Task GetPendingRequests_None_ThrowsNotFound()
        {
            _mockRegRepo.Setup(r => r.GetPendingAsync()).ReturnsAsync(new List<AttendanceRegularization>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPendingRequestsAsync());
        }

        [Fact]
        public async Task Approve_NotFound_ThrowsNotFound()
        {
            _mockRegRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((AttendanceRegularization)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveAsync(999, 1, null));
        }

        [Fact]
        public async Task Approve_AlreadyProcessed_ThrowsBadRequest()
        {
            var req = new AttendanceRegularization { Id = 1, Status = "Approved" };
            _mockRegRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(req);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.ApproveAsync(1, 1, null));
        }

        [Fact]
        public async Task Reject_NotFound_ThrowsNotFound()
        {
            _mockRegRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((AttendanceRegularization)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.RejectAsync(999, 1, null));
        }

        [Fact]
        public async Task Reject_AlreadyProcessed_ThrowsBadRequest()
        {
            var req = new AttendanceRegularization { Id = 1, Status = "Rejected" };
            _mockRegRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(req);
            await Assert.ThrowsAsync<BadRequestException>(() => _service.RejectAsync(1, 1, null));
        }

        [Fact]
        public async Task GetMissedCheckouts_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMissedCheckoutsAsync("bad@t.com"));
        }

        [Fact]
        public async Task GetMissedCheckouts_None_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockAttRepo.Setup(r => r.GetMissedCheckoutsAsync(1)).ReturnsAsync(new List<Attendance>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMissedCheckoutsAsync("t@t.com"));
        }
    }
}


