using Application.DTOs.Notification;
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
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _mockNotifRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<NotificationService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _mockNotifRepo = new Mock<INotificationRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<NotificationService>>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new NotificationService(
                _mockNotifRepo.Object, _mockEmpRepo.Object,
                _mockMapper.Object, _mockLogger.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task GetMyNotifications_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetMyNotificationsAsync("bad@t.com", false));
        }

        [Fact]
        public async Task GetMyNotifications_Valid_ReturnsResult()
        {
            var emp = new Employee { Id = 1 };
            var notifs = new List<Notification> { new Notification { Id = 1, Title = "Test" } };
            var dtos = new List<NotificationResponseDto> { new NotificationResponseDto { Id = 1, Title = "Test" } };

            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockNotifRepo.Setup(r => r.GetByEmployeeIdAsync(1, false)).ReturnsAsync(notifs);
            _mockNotifRepo.Setup(r => r.GetUnreadCountAsync(1)).ReturnsAsync(3);
            _mockMapper.Setup(m => m.Map<List<NotificationResponseDto>>(notifs)).Returns(dtos);

            var result = await _service.GetMyNotificationsAsync("t@t.com", false);

            Assert.Single(result.Notifications);
            Assert.Equal(3, result.UnreadCount);
        }

        [Fact]
        public async Task MarkAsRead_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.MarkAsReadAsync(1, "bad@t.com"));
        }

        [Fact]
        public async Task MarkAsRead_NotificationNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockNotifRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Notification)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.MarkAsReadAsync(999, "t@t.com"));
        }

        [Fact]
        public async Task MarkAsRead_NotOwner_ThrowsUnauthorized()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var notif = new Notification { Id = 1, EmployeeId = 99 };
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);
            await Assert.ThrowsAsync<UnauthorizedException>(() => _service.MarkAsReadAsync(1, "t@t.com"));
        }

        [Fact]
        public async Task MarkAsRead_AlreadyRead_DoesNotSave()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var notif = new Notification { Id = 1, EmployeeId = 1, IsRead = true };
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

            await _service.MarkAsReadAsync(1, "t@t.com");

            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task MarkAsRead_Valid_MarksAndSaves()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var notif = new Notification { Id = 1, EmployeeId = 1, IsRead = false };
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

            await _service.MarkAsReadAsync(1, "t@t.com");

            Assert.True(notif.IsRead);
            Assert.NotNull(notif.ReadAt);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkAllAsRead_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.MarkAllAsReadAsync("bad@t.com"));
        }

        [Fact]
        public async Task MarkAllAsRead_NoUnread_ReturnsZero()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockNotifRepo.Setup(r => r.GetByEmployeeIdAsync(1, true)).ReturnsAsync(new List<Notification>());

            var result = await _service.MarkAllAsReadAsync("t@t.com");

            Assert.Equal(0, result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteNotification_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteNotificationAsync(1, "bad@t.com"));
        }

        [Fact]
        public async Task DeleteNotification_NotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockNotifRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Notification)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteNotificationAsync(999, "t@t.com"));
        }

        [Fact]
        public async Task DeleteNotification_NotOwner_ThrowsUnauthorized()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Notification { Id = 1, EmployeeId = 99 });
            await Assert.ThrowsAsync<UnauthorizedException>(() => _service.DeleteNotificationAsync(1, "t@t.com"));
        }

        [Fact]
        public async Task DeleteNotification_Valid_DeletesAndSaves()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var notif = new Notification { Id = 1, EmployeeId = 1 };
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

            await _service.DeleteNotificationAsync(1, "t@t.com");

            _mockNotifRepo.Verify(r => r.Delete(notif), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Broadcast_SendsToAllActive()
        {
            var emps = new List<Employee>
            {
                new Employee { Id = 1, IsActive = true },
                new Employee { Id = 2, IsActive = true },
                new Employee { Id = 3, IsActive = false }
            };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);

            var dto = new BroadcastNotificationDto { Title = "Hello", Message = "Test" };
            var result = await _service.BroadcastAsync(dto, "admin");

            Assert.Equal(2, result);
            _mockNotifRepo.Verify(r => r.AddRangeAsync(It.Is<List<Notification>>(l => l.Count == 2)), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // MarkAllAsRead Updates All

        [Fact]
        public async Task MarkAllAsRead_MultipleUnread_MarksAllAndReturnsCount()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var unread = new List<Notification>
            {
                new Notification { Id = 1, EmployeeId = 1, IsRead = false },
                new Notification { Id = 2, EmployeeId = 1, IsRead = false },
                new Notification { Id = 3, EmployeeId = 1, IsRead = false }
            };
            _mockNotifRepo.Setup(r => r.GetByEmployeeIdAsync(1, true)).ReturnsAsync(unread);

            var result = await _service.MarkAllAsReadAsync("t@t.com");

            Assert.Equal(3, result);
            Assert.All(unread, n => Assert.True(n.IsRead));
            Assert.All(unread, n => Assert.NotNull(n.ReadAt));
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // Broadcast Only Active Employees

        [Fact]
        public async Task Broadcast_OnlyActiveEmployees_ReceiveNotification()
        {
            var emps = new List<Employee>
            {
                new Employee { Id = 1, IsActive = true },
                new Employee { Id = 2, IsActive = false },
                new Employee { Id = 3, IsActive = true }
            };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);

            List<Notification> capturedNotifs = null;
            _mockNotifRepo.Setup(r => r.AddRangeAsync(It.IsAny<List<Notification>>()))
                .Callback<List<Notification>>(n => capturedNotifs = n)
                .Returns(Task.CompletedTask);

            var dto = new BroadcastNotificationDto { Title = "Announcement", Message = "Holiday on Monday" };
            var result = await _service.BroadcastAsync(dto, "hr_admin");

            Assert.Equal(2, result);
            Assert.NotNull(capturedNotifs);
            Assert.Equal(2, capturedNotifs.Count);
            Assert.All(capturedNotifs, n =>
            {
                Assert.Equal("Announcement", n.Title);
                Assert.Equal("Holiday on Monday", n.Message);
                Assert.Equal("hr_admin", n.CreatedBy);
                Assert.False(n.IsRead);
            });
            Assert.Contains(capturedNotifs, n => n.EmployeeId == 1);
            Assert.Contains(capturedNotifs, n => n.EmployeeId == 3);
            Assert.DoesNotContain(capturedNotifs, n => n.EmployeeId == 2);
        }

        // No Active Employees for Broadcast

        [Fact]
        public async Task Broadcast_NoActiveEmployees_ReturnsZero()
        {
            var emps = new List<Employee>
            {
                new Employee { Id = 1, IsActive = false },
                new Employee { Id = 2, IsActive = false }
            };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);

            var result = await _service.BroadcastAsync(new BroadcastNotificationDto { Title = "T", Message = "M" }, "admin");

            Assert.Equal(0, result);
        }

        // MarkAsRead Sets ReadAt Timestamp

        [Fact]
        public async Task MarkAsRead_Valid_SetsReadAtCloseToNow()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            var notif = new Notification { Id = 1, EmployeeId = 1, IsRead = false, ReadAt = null };
            _mockNotifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

            await _service.MarkAsReadAsync(1, "t@t.com");

            Assert.True(notif.IsRead);
            Assert.NotNull(notif.ReadAt);
            Assert.True((DateTime.Now - notif.ReadAt.Value).TotalSeconds < 5);
        }

        // Notification counts check

        [Fact]
        public async Task GetMyNotifications_OnlyUnread_FiltersCorrectly()
        {
            var emp = new Employee { Id = 1 };
            var unread = new List<Notification>
            {
                new Notification { Id = 1, Title = "Unread1", IsRead = false }
            };
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockNotifRepo.Setup(r => r.GetByEmployeeIdAsync(1, true)).ReturnsAsync(unread);
            _mockNotifRepo.Setup(r => r.GetUnreadCountAsync(1)).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<List<NotificationResponseDto>>(unread))
                .Returns(new List<NotificationResponseDto> { new NotificationResponseDto { Id = 1 } });

            var result = await _service.GetMyNotificationsAsync("t@t.com", true);

            Assert.Single(result.Notifications);
            Assert.Equal(1, result.UnreadCount);
        }
    }
}


