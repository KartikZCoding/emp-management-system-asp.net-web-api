using Application.DTOs.Auth;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly RoleService _service;

        public RoleServiceTests()
        {
            _mockRepo = new Mock<IRoleRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new RoleService(_mockRepo.Object, _mockMapper.Object, _mockUoW.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_EmptyName_ThrowsBadRequest(string name)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateRoleAsync(new RoleDto { RoleName = name }));
        }

        [Fact]
        public async Task Create_DuplicateRole_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateRoleAsync(new RoleDto { RoleName = "Admin" }));
        }

        [Fact]
        public async Task Create_Valid_CreatesAndSaves()
        {
            _mockRepo.Setup(r => r.RoleExistsAsync("NewRole")).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Role>(It.IsAny<RoleDto>())).Returns(new Role());

            await _service.CreateRoleAsync(new RoleDto { RoleName = "NewRole" });

            _mockRepo.Verify(r => r.CreateRoleAsync(It.IsAny<Role>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsMappedList()
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, RoleName = "Admin", Description = "Admin role" },
                new Role { Id = 2, RoleName = "Employee", Description = "Employee role" }
            };
            _mockRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(roles);

            var result = await _service.GetAllRolesAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Admin", result[0].RoleName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetRoleByIdAsync(id));
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetRoleByIdAsync(999)).ReturnsAsync((Role)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetRoleByIdAsync(999));
        }

        [Fact]
        public async Task GetById_Valid_ReturnsDto()
        {
            var role = new Role { Id = 1, RoleName = "Admin", Description = "desc" };
            _mockRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(role);

            var result = await _service.GetRoleByIdAsync(1);

            Assert.Equal("Admin", result.RoleName);
            Assert.Equal(1, result.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateRoleAsync(id, new RoleDto { RoleName = "Test" }));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Update_EmptyName_ThrowsBadRequest(string name)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateRoleAsync(1, new RoleDto { RoleName = name }));
        }

        [Fact]
        public async Task Update_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetRoleByIdAsync(999)).ReturnsAsync((Role)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateRoleAsync(999, new RoleDto { RoleName = "Test" }));
        }

        [Fact]
        public async Task Update_Valid_UpdatesAndSaves()
        {
            var role = new Role { Id = 1 };
            _mockRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(role);

            await _service.UpdateRoleAsync(1, new RoleDto { RoleName = "Updated" });

            _mockRepo.Verify(r => r.UpdateRoleAsync(role), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Delete_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.DeleteRoleAsync(id));
        }

        [Fact]
        public async Task Delete_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetRoleByIdAsync(999)).ReturnsAsync((Role)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteRoleAsync(999));
        }

        [Fact]
        public async Task Delete_Valid_DeletesAndSaves()
        {
            var role = new Role { Id = 1 };
            _mockRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(role);

            await _service.DeleteRoleAsync(1);

            _mockRepo.Verify(r => r.DeleteRoleAsync(role), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}


