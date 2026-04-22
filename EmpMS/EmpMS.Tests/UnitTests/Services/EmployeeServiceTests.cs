using Application.Common;
using Application.DTOs.Employee;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ILogger<EmployeeService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();
            _mockUoW = new Mock<IUnitOfWork>();

            _service = new EmployeeService(
                _mockRepo.Object,
                _mockMapper.Object,
                _mockEnv.Object,
                _mockLogger.Object,
                _mockUoW.Object
            );
        }

        // GetAllEmployeesAsync

        [Fact]
        public async Task GetAllEmployees_ReturnsPagedResult()
        {
            var employees = new List<Employee> { new Employee { Id = 1, FirstName = "Test" } };
            var dtos = new List<EmployeeListDto> { new EmployeeListDto { Id = 1, FullName = "Test" } };

            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(dtos);

            var result = await _service.GetAllEmployeesAsync(1, 10, null, null);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.Page);
        }

        [Fact]
        public async Task GetAllEmployees_InvalidPage_DefaultsToOne()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetAllEmployeesAsync(-1, -5, null, null);

            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task GetAllEmployees_NullFromRepo_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "asc")).ReturnsAsync((List<Employee>)null);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetAllEmployeesAsync(1, 10, null, null));
        }

        [Fact]
        public async Task GetAllEmployees_DescSortOrder_PassesDesc()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, "name", "desc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetAllEmployeesAsync(1, 10, "name", "desc");

            _mockRepo.Verify(r => r.GetAllAsync(1, 10, "name", "desc"), Times.Once);
        }

        // GetEmployeeByIdAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetEmployeeById_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetEmployeeByIdAsync(id));
        }

        [Fact]
        public async Task GetEmployeeById_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetEmployeeByIdAsync(999));
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsDto()
        {
            var employee = new Employee { Id = 1, FirstName = "Kartik" };
            var dto = new EmployeeResponseDto { Id = 1, FirstName = "Kartik" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(dto);

            var result = await _service.GetEmployeeByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Kartik", result.FirstName);
        }

        // CreateEmployeeAsync

        [Fact]
        public async Task CreateEmployee_DuplicateEmail_ThrowsBadRequest()
        {
            _mockRepo.Setup(r => r.EmailExistAsync("existing@test.com")).ReturnsAsync(true);
            var dto = new CreateEmployeeDto { Email = "existing@test.com" };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateEmployeeAsync(dto));
        }

        [Fact]
        public async Task CreateEmployee_ValidData_CallsCreateAndSave()
        {
            _mockRepo.Setup(r => r.EmailExistAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<CreateEmployeeDto>())).Returns(new Employee());

            var dto = new CreateEmployeeDto { Email = "new@test.com", FirstName = "Test" };

            await _service.CreateEmployeeAsync(dto);

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // UpdateEmployeeAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateEmployee_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateEmployeeAsync(id, new UpdateEmployeeDto()));
        }

        [Fact]
        public async Task UpdateEmployee_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateEmployeeAsync(999, new UpdateEmployeeDto()));
        }

        [Fact]
        public async Task UpdateEmployee_EmailTakenByAnother_ThrowsBadRequest()
        {
            var employee = new Employee { Id = 1, Email = "original@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepo.Setup(r => r.EmailExistAsync("taken@test.com")).ReturnsAsync(true);

            var dto = new UpdateEmployeeDto { Email = "taken@test.com" };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateEmployeeAsync(1, dto));
        }

        [Fact]
        public async Task UpdateEmployee_SameEmail_DoesNotCheckDuplicate()
        {
            var employee = new Employee { Id = 1, Email = "same@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

            var dto = new UpdateEmployeeDto { Email = "same@test.com" };

            await _service.UpdateEmployeeAsync(1, dto);

            _mockRepo.Verify(r => r.EmailExistAsync(It.IsAny<string>()), Times.Never);
            _mockRepo.Verify(r => r.UpdateAsync(employee), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ValidData_UpdatesAndSaves()
        {
            var employee = new Employee { Id = 1, Email = "old@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepo.Setup(r => r.EmailExistAsync("new@test.com")).ReturnsAsync(false);

            var dto = new UpdateEmployeeDto { Email = "new@test.com" };

            await _service.UpdateEmployeeAsync(1, dto);

            _mockRepo.Verify(r => r.UpdateAsync(employee), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // SoftDeleteEmployeeAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SoftDelete_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.SoftDeleteEmployeeAsync(id));
        }

        [Fact]
        public async Task SoftDelete_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.SoftDeleteEmployeeAsync(999));
        }

        [Fact]
        public async Task SoftDelete_ValidEmployee_DeletesAndSaves()
        {
            var employee = new Employee { Id = 5 };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(employee);

            await _service.SoftDeleteEmployeeAsync(5);

            _mockRepo.Verify(r => r.SoftDeleteAsync(employee), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // SearchEmployeesAsync

        [Fact]
        public async Task SearchEmployees_ReturnsMappedResults()
        {
            var employees = new List<Employee> { new Employee { Id = 1 } };
            var dtos = new List<EmployeeListDto> { new EmployeeListDto { Id = 1 } };

            _mockRepo.Setup(r => r.SearchAsync("test", null, null)).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(dtos);

            var result = await _service.SearchEmployeesAsync("test", null, null);

            Assert.Single(result);
        }

        // GetByDepartmentAsync

        [Fact]
        public async Task GetByDepartment_ReturnsMappedList()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetByDepartmentAsync(1)).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetByDepartmentAsync(1);

            Assert.NotNull(result);
        }

        // GetByManagerAsync

        [Fact]
        public async Task GetByManager_ReturnsMappedList()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetByManagerAsync(1)).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetByManagerAsync(1);

            Assert.NotNull(result);
        }

        // GetOwnProfileAsync

        [Fact]
        public async Task GetOwnProfile_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync("bad@test.com")).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetOwnProfileAsync("bad@test.com"));
        }

        [Fact]
        public async Task GetOwnProfile_ValidEmail_ReturnsDto()
        {
            var employee = new Employee { Id = 1, Email = "test@test.com" };
            var dto = new EmployeeResponseDto { Id = 1 };

            _mockRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(dto);

            var result = await _service.GetOwnProfileAsync("test@test.com");

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        // UpdateOwnProfileAsync

        [Fact]
        public async Task UpdateOwnProfile_NotFound_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync("bad@test.com")).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateOwnProfileAsync("bad@test.com", new UpdateOwnProfileDto()));
        }

        [Fact]
        public async Task UpdateOwnProfile_Valid_UpdatesAndSaves()
        {
            var employee = new Employee { Id = 1, Email = "test@test.com" };
            _mockRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(employee);

            await _service.UpdateOwnProfileAsync("test@test.com", new UpdateOwnProfileDto());

            _mockRepo.Verify(r => r.UpdateAsync(employee), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // Pagination Math

        [Fact]
        public async Task GetAllEmployees_25Records_CalculatesTotalPagesCorrectly()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(25);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetAllEmployeesAsync(1, 10, null, null);

            Assert.Equal(3, result.TotalPages);
            Assert.True(result.HasNextPage);
            Assert.False(result.HasPreviousPage);
        }

        [Fact]
        public async Task GetAllEmployees_Page2Of3_HasBothNavigation()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(2, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(25);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetAllEmployeesAsync(2, 10, null, null);

            Assert.True(result.HasPreviousPage);
            Assert.True(result.HasNextPage);
        }

        [Fact]
        public async Task GetAllEmployees_LastPage_NoNextPage()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(3, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(25);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            var result = await _service.GetAllEmployeesAsync(3, 10, null, null);

            Assert.True(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
        }

        // Sort Order Normalization

        [Theory]
        [InlineData("DESC")]
        [InlineData("Desc")]
        [InlineData("dEsC")]
        public async Task GetAllEmployees_AnyCaseDesc_NormalizesToDesc(string sortOrder)
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "desc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            await _service.GetAllEmployeesAsync(1, 10, null, sortOrder);

            _mockRepo.Verify(r => r.GetAllAsync(1, 10, null, "desc"), Times.Once);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("ascending")]
        [InlineData("xyz")]
        public async Task GetAllEmployees_InvalidSortOrder_DefaultsToAsc(string sortOrder)
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.GetAllAsync(1, 10, null, "asc")).ReturnsAsync(employees);
            _mockRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            await _service.GetAllEmployeesAsync(1, 10, null, sortOrder);

            _mockRepo.Verify(r => r.GetAllAsync(1, 10, null, "asc"), Times.Once);
        }

        // Audit Fields on Create

        [Fact]
        public async Task CreateEmployee_SetsCreatedAtAndIsActive()
        {
            Employee capturedEmployee = null;
            _mockRepo.Setup(r => r.EmailExistAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<CreateEmployeeDto>())).Returns(new Employee());
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
                .Callback<Employee>(e => capturedEmployee = e)
                .Returns(Task.CompletedTask);

            await _service.CreateEmployeeAsync(new CreateEmployeeDto { Email = "new@test.com" });

            Assert.NotNull(capturedEmployee);
            Assert.True(capturedEmployee.IsActive);
            Assert.True((DateTime.Now - capturedEmployee.CreatedAt).TotalSeconds < 5);
        }

        // Audit Fields on Update

        [Fact]
        public async Task UpdateEmployee_SetsUpdatedAtTimestamp()
        {
            var employee = new Employee { Id = 1, Email = "old@test.com", UpdatedAt = null };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepo.Setup(r => r.EmailExistAsync("new@test.com")).ReturnsAsync(false);

            await _service.UpdateEmployeeAsync(1, new UpdateEmployeeDto { Email = "new@test.com" });

            Assert.NotNull(employee.UpdatedAt);
            Assert.True((DateTime.Now - employee.UpdatedAt.Value).TotalSeconds < 5);
        }

        // Operation Order Verification

        [Fact]
        public async Task CreateEmployee_SaveIsCalledAfterCreate()
        {
            var callOrder = new List<string>();

            _mockRepo.Setup(r => r.EmailExistAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<CreateEmployeeDto>())).Returns(new Employee());
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
                .Callback(() => callOrder.Add("Create"))
                .Returns(Task.CompletedTask);
            _mockUoW.Setup(u => u.SaveChangesAsync())
                .Callback(() => callOrder.Add("Save"))
                .ReturnsAsync(1);

            await _service.CreateEmployeeAsync(new CreateEmployeeDto { Email = "new@test.com" });

            Assert.Equal(new[] { "Create", "Save" }, callOrder);
        }

        // Email change with new unique email

        [Fact]
        public async Task UpdateEmployee_NewUniqueEmail_ChecksDuplicateAndUpdates()
        {
            var employee = new Employee { Id = 1, Email = "old@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepo.Setup(r => r.EmailExistAsync("available@test.com")).ReturnsAsync(false);

            await _service.UpdateEmployeeAsync(1, new UpdateEmployeeDto { Email = "available@test.com" });

            _mockRepo.Verify(r => r.EmailExistAsync("available@test.com"), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(employee), Times.Once);
        }

        // Mapper called with correct entity

        [Fact]
        public async Task UpdateOwnProfile_MapperCalledWithCorrectArgs()
        {
            var employee = new Employee { Id = 1, Email = "t@test.com" };
            var profileDto = new UpdateOwnProfileDto { Phone = "9999999999", Address = "New Address" };

            _mockRepo.Setup(r => r.GetByEmailAsync("t@test.com")).ReturnsAsync(employee);

            await _service.UpdateOwnProfileAsync("t@test.com", profileDto);

            _mockMapper.Verify(m => m.Map(profileDto, employee), Times.Once);
        }

        [Fact]
        public async Task UpdateOwnProfile_SetsUpdatedAtTimestamp()
        {
            var employee = new Employee { Id = 1, Email = "t@test.com", UpdatedAt = null };
            _mockRepo.Setup(r => r.GetByEmailAsync("t@test.com")).ReturnsAsync(employee);

            await _service.UpdateOwnProfileAsync("t@test.com", new UpdateOwnProfileDto());

            Assert.NotNull(employee.UpdatedAt);
        }

        // Search with multiple filters

        [Fact]
        public async Task SearchEmployees_WithAllFilters_PassesCorrectArgs()
        {
            var employees = new List<Employee>();
            _mockRepo.Setup(r => r.SearchAsync("kartik", 2, 5)).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(employees)).Returns(new List<EmployeeListDto>());

            await _service.SearchEmployeesAsync("kartik", 2, 5);

            _mockRepo.Verify(r => r.SearchAsync("kartik", 2, 5), Times.Once);
        }

        [Fact]
        public async Task SearchEmployees_NoResults_ReturnsEmptyList()
        {
            _mockRepo.Setup(r => r.SearchAsync("nonexistent", null, null)).ReturnsAsync(new List<Employee>());
            _mockMapper.Setup(m => m.Map<List<EmployeeListDto>>(It.IsAny<List<Employee>>())).Returns(new List<EmployeeListDto>());

            var result = await _service.SearchEmployeesAsync("nonexistent", null, null);

            Assert.Empty(result);
        }

        // GetById does NOT call repo for invalid IDs

        [Fact]
        public async Task GetEmployeeById_InvalidId_DoesNotHitDatabase()
        {
            await Assert.ThrowsAsync<BadRequestException>(() => _service.GetEmployeeByIdAsync(-1));

            _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }
    }
}


