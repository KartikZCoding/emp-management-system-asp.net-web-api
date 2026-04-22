using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class PrivilegeServiceTests
    {
        private readonly Mock<IPrivilegeRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly PrivilegeService _service;

        public PrivilegeServiceTests()
        {
            _mockRepo = new Mock<IPrivilegeRepository>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new PrivilegeService(_mockRepo.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsMappedList()
        {
            var privileges = new List<Privilege>
            {
                new Privilege { Id = 1, PrivilegeName = "ReadEmployees", Description = "Read" }
            };
            _mockRepo.Setup(r => r.GetAllPrivilegesAsync()).ReturnsAsync(privileges);

            var result = await _service.GetAllPrivilegesAsync();

            Assert.Single(result);
            Assert.Equal("ReadEmployees", result[0].PrivilegeName);
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(999)).ReturnsAsync((Privilege)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetPrivilegeByIdAsync(999));
        }

        [Fact]
        public async Task GetById_Valid_ReturnsDto()
        {
            var privilege = new Privilege { Id = 1, PrivilegeName = "Write", Description = "desc" };
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);

            var result = await _service.GetPrivilegeByIdAsync(1);

            Assert.Equal("Write", result.PrivilegeName);
        }

        [Fact]
        public async Task Create_Duplicate_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.PrivilegeExistsAsync("Read")).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreatePrivilegeAsync(new PrivilegeDto { PrivilegeName = "Read" }));
        }

        [Fact]
        public async Task Create_Valid_CreatesAndSaves()
        {
            _mockRepo.Setup(r => r.PrivilegeExistsAsync("NewPriv")).ReturnsAsync(false);

            await _service.CreatePrivilegeAsync(new PrivilegeDto { PrivilegeName = "NewPriv", Description = "desc" });

            _mockRepo.Verify(r => r.CreatePrivilegeAsync(It.IsAny<Privilege>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(999)).ReturnsAsync((Privilege)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdatePrivilegeAsync(999, new PrivilegeDto { PrivilegeName = "Test" }));
        }

        [Fact]
        public async Task Update_DuplicateName_ThrowsBadRequest()
        {
            var privilege = new Privilege { Id = 1, PrivilegeName = "Old" };
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockRepo.Setup(r => r.PrivilegeExistsAsync("Taken")).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdatePrivilegeAsync(1, new PrivilegeDto { PrivilegeName = "Taken" }));
        }

        [Fact]
        public async Task Update_Valid_UpdatesAndSaves()
        {
            var privilege = new Privilege { Id = 1, PrivilegeName = "Old" };
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockRepo.Setup(r => r.PrivilegeExistsAsync("Updated")).ReturnsAsync(false);

            await _service.UpdatePrivilegeAsync(1, new PrivilegeDto { PrivilegeName = "Updated", Description = "new" });

            Assert.Equal("Updated", privilege.PrivilegeName);
            _mockRepo.Verify(r => r.UpdatePrivilegeAsync(privilege), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(999)).ReturnsAsync((Privilege)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeletePrivilegeAsync(999));
        }

        [Fact]
        public async Task Delete_Valid_DeletesAndSaves()
        {
            var privilege = new Privilege { Id = 1 };
            _mockRepo.Setup(r => r.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);

            await _service.DeletePrivilegeAsync(1);

            _mockRepo.Verify(r => r.DeletePrivilegeAsync(privilege), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}


