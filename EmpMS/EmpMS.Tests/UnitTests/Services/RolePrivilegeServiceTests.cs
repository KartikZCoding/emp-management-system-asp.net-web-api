using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class RolePrivilegeServiceTests
    {
        private readonly Mock<IRolePrivilegeRepository> _mockRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly Mock<IPrivilegeRepository> _mockPrivRepo;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly RolePrivilegeService _service;

        public RolePrivilegeServiceTests()
        {
            _mockRepo = new Mock<IRolePrivilegeRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockPrivRepo = new Mock<IPrivilegeRepository>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new RolePrivilegeService(
                _mockRepo.Object, _mockRoleRepo.Object,
                _mockPrivRepo.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task Assign_RoleNotFound_ThrowsNotFound()
        {
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(999)).ReturnsAsync((Role)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.AssignPrivilegeToRoleAsync(new RolePrivilegeDto { RoleId = 999, PrivilegeId = 1 }));
        }

        [Fact]
        public async Task Assign_PrivilegeNotFound_ThrowsNotFound()
        {
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1 });
            _mockPrivRepo.Setup(r => r.GetPrivilegeByIdAsync(999)).ReturnsAsync((Privilege)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.AssignPrivilegeToRoleAsync(new RolePrivilegeDto { RoleId = 1, PrivilegeId = 999 }));
        }

        [Fact]
        public async Task Assign_AlreadyAssigned_ThrowsBadRequest()
        {
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1 });
            _mockPrivRepo.Setup(r => r.GetPrivilegeByIdAsync(1)).ReturnsAsync(new Privilege { Id = 1 });
            _mockRepo.Setup(r => r.RolePrivilegeExistsAsync(1, 1)).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.AssignPrivilegeToRoleAsync(new RolePrivilegeDto { RoleId = 1, PrivilegeId = 1 }));
        }

        [Fact]
        public async Task Assign_Valid_AddsAndSaves()
        {
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1 });
            _mockPrivRepo.Setup(r => r.GetPrivilegeByIdAsync(2)).ReturnsAsync(new Privilege { Id = 2 });
            _mockRepo.Setup(r => r.RolePrivilegeExistsAsync(1, 2)).ReturnsAsync(false);

            await _service.AssignPrivilegeToRoleAsync(new RolePrivilegeDto { RoleId = 1, PrivilegeId = 2 });

            _mockRepo.Verify(r => r.AddRolePrivilegeAsync(It.IsAny<RolePrivilege>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPrivsByRole_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetPrivilegesByRoleIdAsync(id));
        }

        [Fact]
        public async Task GetPrivsByRole_EmptyList_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetPrivilegesByRoleIdAsync(1)).ReturnsAsync(new List<Privilege>());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetPrivilegesByRoleIdAsync(1));
        }

        [Fact]
        public async Task GetPrivsByRole_Valid_ReturnsList()
        {
            var privs = new List<Privilege>
            {
                new Privilege { Id = 1, PrivilegeName = "Read", Description = "Read access" }
            };
            _mockRepo.Setup(r => r.GetPrivilegesByRoleIdAsync(1)).ReturnsAsync(privs);

            var result = await _service.GetPrivilegesByRoleIdAsync(1);

            Assert.Single(result);
            Assert.Equal("Read", result[0].PrivilegeName);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public async Task Remove_InvalidIds_ThrowsBadRequest(int roleId, int privId)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.RemovePrivilegeFromRoleAsync(roleId, privId));
        }

        [Fact]
        public async Task Remove_LinkNotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetRolePrivilegeAsync(1, 1)).ReturnsAsync((RolePrivilege)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.RemovePrivilegeFromRoleAsync(1, 1));
        }

        [Fact]
        public async Task Remove_Valid_DeletesAndSaves()
        {
            var rp = new RolePrivilege { RoleId = 1, PrivilegeId = 1 };
            _mockRepo.Setup(r => r.GetRolePrivilegeAsync(1, 1)).ReturnsAsync(rp);

            await _service.RemovePrivilegeFromRoleAsync(1, 1);

            _mockRepo.Verify(r => r.DeleteRolePrivilegeAsync(rp), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}


