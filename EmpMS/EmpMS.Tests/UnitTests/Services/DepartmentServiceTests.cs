using Application.DTOs.Department;
using Application.DTOs.Employee;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _mockRepo = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new DepartmentService(_mockRepo.Object, _mockMapper.Object, _mockUoW.Object);
        }

        // GetAllDepartmentsAsync

        [Fact]
        public async Task GetAll_ReturnsMappedList()
        {
            var departments = new List<Department> { new Department { Id = 1, DepartmentName = "HR" } };
            var dtos = new List<DepartmentResponseDto> { new DepartmentResponseDto { Id = 1, DepartmentName = "HR" } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(departments);
            _mockMapper.Setup(m => m.Map<List<DepartmentResponseDto>>(departments)).Returns(dtos);

            var result = await _service.GetAllDepartmentsAsync();

            Assert.Single(result);
            Assert.Equal("HR", result[0].DepartmentName);
        }

        // GetDepartmentByIdAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetDepartmentByIdAsync(id));
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetDepartmentByIdAsync(999));
        }

        [Fact]
        public async Task GetById_Valid_ReturnsDto()
        {
            var dept = new Department { Id = 1, DepartmentName = "Engineering" };
            var dto = new DepartmentResponseDto { Id = 1, DepartmentName = "Engineering" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);
            _mockMapper.Setup(m => m.Map<DepartmentResponseDto>(dept)).Returns(dto);

            var result = await _service.GetDepartmentByIdAsync(1);

            Assert.Equal("Engineering", result.DepartmentName);
        }

        // CreateDepartmentAsync

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_EmptyName_ThrowsBadRequest(string name)
        {
            var dto = new DepartmentDto { DepartmentName = name };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDepartmentAsync(dto));
        }

        [Fact]
        public async Task Create_DuplicateName_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.ExistsAsync("HR")).ReturnsAsync(true);
            var dto = new DepartmentDto { DepartmentName = "HR" };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDepartmentAsync(dto));
        }

        [Fact]
        public async Task Create_ValidData_CreatesAndSaves()
        {
            _mockRepo.Setup(r => r.ExistsAsync("NewDept")).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Department>(It.IsAny<DepartmentDto>())).Returns(new Department());

            var dto = new DepartmentDto { DepartmentName = "NewDept" };
            await _service.CreateDepartmentAsync(dto);

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Department>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // UpdateDepartmentAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateDepartmentAsync(id, new DepartmentDto { DepartmentName = "Test" }));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Update_EmptyName_ThrowsBadRequest(string name)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateDepartmentAsync(1, new DepartmentDto { DepartmentName = name }));
        }

        [Fact]
        public async Task Update_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateDepartmentAsync(999, new DepartmentDto { DepartmentName = "Test" }));
        }

        [Fact]
        public async Task Update_Valid_UpdatesAndSaves()
        {
            var dept = new Department { Id = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);

            await _service.UpdateDepartmentAsync(1, new DepartmentDto { DepartmentName = "Updated" });

            _mockRepo.Verify(r => r.UpdateAsync(dept), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // DeleteDepartmentAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Delete_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.DeleteDepartmentAsync(id));
        }

        [Fact]
        public async Task Delete_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteDepartmentAsync(999));
        }

        [Fact]
        public async Task Delete_Valid_SoftDeletesAndSaves()
        {
            var dept = new Department { Id = 1, IsActive = true };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);

            await _service.DeleteDepartmentAsync(1);

            Assert.False(dept.IsActive);
            _mockRepo.Verify(r => r.DeleteAsync(dept), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // GetEmployeesInDepartmentAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetEmployeesInDept_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetEmployeesInDepartmentAsync(id));
        }

        [Fact]
        public async Task GetEmployeesInDept_DeptNotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetEmployeesInDepartmentAsync(999));
        }

        [Fact]
        public async Task GetEmployeesInDept_Valid_ReturnsMappedList()
        {
            var dept = new Department { Id = 1 };
            var employees = new List<Employee> { new Employee { Id = 1 } };
            var dtos = new List<EmployeeListDto> { new EmployeeListDto { Id = 1 } };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);
            _mockRepo.Setup(r => r.GetEmployeesByDepartmentIdAsync(1)).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(dtos);

            var result = await _service.GetEmployeesInDepartmentAsync(1);

            Assert.Single(result);
        }
    }
}


