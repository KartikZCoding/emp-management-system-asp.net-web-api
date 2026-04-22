using Application.DTOs.Designation;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class DesignationServiceTests
    {
        private readonly Mock<IDesignationRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly DesignationService _service;

        public DesignationServiceTests()
        {
            _mockRepo = new Mock<IDesignationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new DesignationService(_mockRepo.Object, _mockMapper.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsMappedList()
        {
            var designations = new List<Designation> { new Designation { Id = 1, DesignationName = "Developer" } };
            var dtos = new List<DesignationResponseDto> { new DesignationResponseDto { Id = 1, DesignationName = "Developer" } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(designations);
            _mockMapper.Setup(m => m.Map<List<DesignationResponseDto>>(designations)).Returns(dtos);

            var result = await _service.GetAllDesignationsAsync();

            Assert.Single(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetDesignationByIdAsync(id));
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Designation)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetDesignationByIdAsync(999));
        }

        [Fact]
        public async Task GetById_Valid_ReturnsDto()
        {
            var designation = new Designation { Id = 1, DesignationName = "Manager" };
            var dto = new DesignationResponseDto { Id = 1, DesignationName = "Manager" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(designation);
            _mockMapper.Setup(m => m.Map<DesignationResponseDto>(designation)).Returns(dto);

            var result = await _service.GetDesignationByIdAsync(1);

            Assert.Equal("Manager", result.DesignationName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_EmptyName_ThrowsBadRequest(string name)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDesignationAsync(new DesignationDto { DesignationName = name }));
        }

        [Fact]
        public async Task Create_DuplicateName_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.ExistsAsync("Developer")).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDesignationAsync(new DesignationDto { DesignationName = "Developer" }));
        }

        [Fact]
        public async Task Create_Valid_CreatesAndSaves()
        {
            _mockRepo.Setup(r => r.ExistsAsync("NewRole")).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Designation>(It.IsAny<DesignationDto>())).Returns(new Designation());

            await _service.CreateDesignationAsync(new DesignationDto { DesignationName = "NewRole" });

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Designation>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateDesignationAsync(id, new DesignationDto { DesignationName = "Test" }));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_EmptyName_ThrowsBadRequest(string name)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateDesignationAsync(1, new DesignationDto { DesignationName = name }));
        }

        [Fact]
        public async Task Update_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Designation)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateDesignationAsync(999, new DesignationDto { DesignationName = "Test" }));
        }

        [Fact]
        public async Task Update_Valid_UpdatesAndSaves()
        {
            var designation = new Designation { Id = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(designation);

            await _service.UpdateDesignationAsync(1, new DesignationDto { DesignationName = "Updated" });

            _mockRepo.Verify(r => r.UpdateAsync(designation), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Delete_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.DeleteDesignationAsync(id));
        }

        [Fact]
        public async Task Delete_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Designation)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteDesignationAsync(999));
        }

        [Fact]
        public async Task Delete_Valid_SoftDeletesAndSaves()
        {
            var designation = new Designation { Id = 1, IsActive = true };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(designation);

            await _service.DeleteDesignationAsync(1);

            Assert.False(designation.IsActive);
            _mockRepo.Verify(r => r.DeleteAsync(designation), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}


