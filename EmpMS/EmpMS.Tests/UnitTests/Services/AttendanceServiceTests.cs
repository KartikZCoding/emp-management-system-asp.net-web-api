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
    public class AttendanceServiceTests
    {
        private readonly Mock<IAttendanceRepository> _mockAttRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AttendanceService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly AttendanceService _service;

        public AttendanceServiceTests()
        {
            _mockAttRepo = new Mock<IAttendanceRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AttendanceService>>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new AttendanceService(
                _mockAttRepo.Object, _mockEmpRepo.Object,
                _mockMapper.Object, _mockLogger.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task CheckIn_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CheckInAsync("bad@t.com"));
        }

        [Fact]
        public async Task CheckIn_AlreadyCheckedIn_ThrowsBadRequest()
        {
            var emp = new Employee { Id = 1, Email = "t@t.com" };
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);

            var att = new Attendance
            {
                Id = 1,
                EmployeeId = 1,
                AttendanceLogs = new List<AttendanceLog>
                {
                    new AttendanceLog { CheckIn = DateTime.Now, CheckOut = null }
                }
            };
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync(att);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.CheckInAsync("t@t.com"));
        }

        [Fact]
        public async Task CheckOut_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CheckOutAsync("bad@t.com"));
        }

        [Fact]
        public async Task CheckOut_NotCheckedIn_ThrowsBadRequest()
        {
            var emp = new Employee { Id = 1 };
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync((Attendance)null);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.CheckOutAsync("t@t.com"));
        }

        [Fact]
        public async Task GetDepartmentAttendance_NoRecords_ThrowsNotFound()
        {
            _mockAttRepo.Setup(r => r.GetByDepartmentAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync(new List<Attendance>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetDepartmentAttendanceAsync(1, null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetEmployeeAttendance_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetEmployeeAttendanceAsync(id, null, null));
        }

        [Fact]
        public async Task GetEmployeeAttendance_NoRecords_ThrowsNotFound()
        {
            _mockAttRepo.Setup(r => r.GetByEmployeeMonthlyAsync(1, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Attendance>());
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetEmployeeAttendanceAsync(1, null, null));
        }

        [Fact]
        public async Task GetMonthlyReport_NoRecords_ThrowsNotFound()
        {
            _mockAttRepo.Setup(r => r.GetMonthlyAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Attendance>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMonthlyReportAsync(null, null));
        }

        [Fact]
        public async Task GetMyAttendance_EmptyEmail_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetMyAttendanceAsync("", null, null));
        }

        [Fact]
        public async Task UpdateAttendance_InvalidId_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateAttendanceAsync(0, new AttendanceUpdateDto { Status = "Present" }, "admin"));
        }

        [Fact]
        public async Task UpdateAttendance_InvalidStatus_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateAttendanceAsync(1, new AttendanceUpdateDto { Status = "InvalidStatus" }, "admin"));
        }

        [Fact]
        public async Task UpdateAttendance_NotFound_ThrowsNotFound()
        {
            _mockAttRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Attendance)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateAttendanceAsync(999, new AttendanceUpdateDto { Status = "Present" }, "admin"));
        }

        [Fact]
        public async Task UpdateAttendance_Valid_UpdatesAndSaves()
        {
            var att = new Attendance { Id = 1, Status = "HalfDay" };
            _mockAttRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(att);
            _mockMapper.Setup(m => m.Map<AttendanceResponseDto>(att)).Returns(new AttendanceResponseDto());

            var result = await _service.UpdateAttendanceAsync(1, new AttendanceUpdateDto { Status = "Present" }, "admin");

            Assert.Equal("Present", att.Status);
            Assert.Equal("admin", att.UpdatedBy);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // First CheckIn Creates New Attendance

        [Fact]
        public async Task CheckIn_FirstTimeToday_CreatesNewAttendanceRecord()
        {
            var emp = new Employee { Id = 1 };
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync((Attendance)null);

            Attendance captured = null;
            _mockAttRepo.Setup(r => r.CreateAsync(It.IsAny<Attendance>()))
                .Callback<Attendance>(a => captured = a)
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<AttendanceResponseDto>(It.IsAny<Attendance>())).Returns(new AttendanceResponseDto());

            await _service.CheckInAsync("t@t.com");

            Assert.NotNull(captured);
            Assert.Equal(1, captured.EmployeeId);
            Assert.Equal("Present", captured.Status);
            Assert.True(captured.IsCheckedIn);
            _mockAttRepo.Verify(r => r.CreateLogAsync(It.IsAny<AttendanceLog>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // CheckOut Sets Duration

        [Fact]
        public async Task CheckOut_Valid_SetsCheckOutTimeAndUpdatesHours()
        {
            var emp = new Employee { Id = 1 };
            var checkInTime = DateTime.Now.AddHours(-8);
            var att = new Attendance
            {
                Id = 1, EmployeeId = 1, TotalHours = 0,
                AttendanceLogs = new List<AttendanceLog>
                {
                    new AttendanceLog { CheckIn = checkInTime, CheckOut = null }
                }
            };

            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync(att);
            _mockMapper.Setup(m => m.Map<AttendanceResponseDto>(att)).Returns(new AttendanceResponseDto());

            await _service.CheckOutAsync("t@t.com");

            Assert.NotNull(att.AttendanceLogs.First().CheckOut);
            Assert.True(att.TotalHours >= 7.5m);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // Valid Status Values Accepted

        [Theory]
        [InlineData("Present")]
        [InlineData("HalfDay")]
        [InlineData("OnLeave")]
        public async Task UpdateAttendance_ValidStatuses_Accepted(string status)
        {
            var att = new Attendance { Id = 1, Status = "Absent" };
            _mockAttRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(att);
            _mockMapper.Setup(m => m.Map<AttendanceResponseDto>(att)).Returns(new AttendanceResponseDto());

            var result = await _service.UpdateAttendanceAsync(1, new AttendanceUpdateDto { Status = status }, "admin");

            Assert.Equal(status, att.Status);
        }

        // ReCheckIn After Checkout

        [Fact]
        public async Task CheckIn_SecondSession_CreatesNewLogViaRepo()
        {
            var emp = new Employee { Id = 1 };
            var att = new Attendance
            {
                Id = 1, EmployeeId = 1,
                AttendanceLogs = new List<AttendanceLog>
                {
                    new AttendanceLog { CheckIn = DateTime.Now.AddHours(-5), CheckOut = DateTime.Now.AddHours(-1) }
                }
            };

            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockAttRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateOnly>())).ReturnsAsync(att);
            _mockMapper.Setup(m => m.Map<AttendanceResponseDto>(att)).Returns(new AttendanceResponseDto());

            await _service.CheckInAsync("t@t.com");

            Assert.True(att.IsCheckedIn);
            Assert.Equal("Present", att.Status);
            _mockAttRepo.Verify(r => r.CreateLogAsync(It.IsAny<AttendanceLog>()), Times.Once);
            _mockAttRepo.Verify(r => r.UpdateAsync(att), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // GetMyAttendance valid flow

        [Fact]
        public async Task GetMyAttendance_ValidEmployee_ReturnsMappedList()
        {
            var emp = new Employee { Id = 1 };
            var atts = new List<Attendance>
            {
                new Attendance { Id = 1, Status = "Present", AttendanceLogs = new List<AttendanceLog>() },
                new Attendance { Id = 2, Status = "Absent", AttendanceLogs = new List<AttendanceLog>() }
            };
            var dtos = new List<AttendanceResponseDto>
            {
                new AttendanceResponseDto { Id = 1 },
                new AttendanceResponseDto { Id = 2 }
            };

            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockAttRepo.Setup(r => r.GetByEmployeeMonthlyAsync(1, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(atts);
            _mockMapper.Setup(m => m.Map<List<AttendanceResponseDto>>(atts)).Returns(dtos);

            var result = await _service.GetMyAttendanceAsync("t@t.com", null, null);

            Assert.Equal(2, result.Count);
        }
    }
}


