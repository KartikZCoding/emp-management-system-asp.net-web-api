using Application.DTOs.Salary;
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
    public class SalaryServiceTests
    {
        private readonly Mock<ISalaryRepository> _mockSalaryRepo;
        private readonly Mock<ISalaryStructureRepository> _mockStructRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IAttendanceRepository> _mockAttRepo;
        private readonly Mock<ILeaveRepository> _mockLeaveRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<SalaryService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly SalaryService _service;

        public SalaryServiceTests()
        {
            _mockSalaryRepo = new Mock<ISalaryRepository>();
            _mockStructRepo = new Mock<ISalaryStructureRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockAttRepo = new Mock<IAttendanceRepository>();
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SalaryService>>();
            _mockUoW = new Mock<IUnitOfWork>();
            _service = new SalaryService(
                _mockSalaryRepo.Object, _mockStructRepo.Object, _mockEmpRepo.Object,
                _mockAttRepo.Object, _mockLeaveRepo.Object, _mockMapper.Object,
                _mockLogger.Object, _mockUoW.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(-1)]
        public async Task GenerateSalary_InvalidMonth_ThrowsBadRequest(int month)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GenerateMonthlySalaryAsync(month, 2025, "admin"));
        }

        [Theory]
        [InlineData(2019)]
        public async Task GenerateSalary_InvalidYear_ThrowsBadRequest(int year)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GenerateMonthlySalaryAsync(1, year, "admin"));
        }

        [Fact]
        public async Task GenerateSalary_FutureMonth_ThrowsBadRequest()
        {
            var futureMonth = DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1;
            var year = DateTime.Now.Month == 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GenerateMonthlySalaryAsync(futureMonth, year, "admin"));
        }

        [Fact]
        public async Task GenerateSalary_NoActiveEmployees_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(new List<Employee>());
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GenerateMonthlySalaryAsync(1, 2025, "admin"));
        }

        [Fact]
        public async Task GenerateSalary_NoSalaryStructure_ThrowsNotFound()
        {
            var emps = new List<Employee> { new Employee { Id = 1, IsActive = true } };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);
            _mockStructRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<SalaryStructure>());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GenerateMonthlySalaryAsync(1, 2025, "admin"));
        }

        [Fact]
        public async Task GetMySalary_EmployeeNotFound_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("bad@t.com")).ReturnsAsync((Employee)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetMySalaryAsync("bad@t.com", 1, 2025));
        }

        [Fact]
        public async Task GetMySalary_NoSalaryRecord_ThrowsNotFound()
        {
            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(new Employee { Id = 1 });
            _mockSalaryRepo.Setup(r => r.GetByEmployeeMonthYearAsync(1, 1, 2025)).ReturnsAsync((Salary)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetMySalaryAsync("t@t.com", 1, 2025));
        }

        [Fact]
        public async Task GetMySalary_Valid_ReturnsDto()
        {
            var emp = new Employee { Id = 1 };
            var sal = new Salary { Id = 1, EmployeeId = 1, NetSalary = 50000 };
            var dto = new SalaryResponseDto { Id = 1, NetSalary = 50000 };

            _mockEmpRepo.Setup(r => r.GetByEmailAsync("t@t.com")).ReturnsAsync(emp);
            _mockSalaryRepo.Setup(r => r.GetByEmployeeMonthYearAsync(1, 1, 2025)).ReturnsAsync(sal);
            _mockMapper.Setup(m => m.Map<SalaryResponseDto>(sal)).Returns(dto);

            var result = await _service.GetMySalaryAsync("t@t.com", 1, 2025);
            Assert.Equal(50000, result.NetSalary);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetEmployeeSalary_InvalidId_ThrowsBadRequest(int id)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GetEmployeeSalaryAsync(id, null, null));
        }

        [Fact]
        public async Task GetEmployeeSalary_NoRecords_ThrowsNotFound()
        {
            _mockSalaryRepo.Setup(r => r.GetByEmployeeAsync(1, null, null)).ReturnsAsync(new List<Salary>());
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetEmployeeSalaryAsync(1, null, null));
        }

        [Fact]
        public async Task GetAllSalaries_NoRecords_ThrowsNotFound()
        {
            _mockSalaryRepo.Setup(r => r.GetAllByMonthYearAsync(1, 2025)).ReturnsAsync(new List<Salary>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllSalariesAsync(1, 2025));
        }

        [Fact]
        public async Task UpdateSalary_NotFound_ThrowsNotFound()
        {
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Salary)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateSalaryAsync(999, new SalaryUpdateDto(), "admin"));
        }

        [Fact]
        public async Task UpdateSalary_WithBonus_RecalculatesCorrectly()
        {
            var sal = new Salary
            {
                Id = 1, Basic = 20000, HRA = 10000, DA = 2000,
                TravelAllowance = 1600, SpecialAllowance = 5000, Bonus = 0,
                EmployeePF = 2400, ProfessionalTax = 200, IncomeTax = 1000, LopDeduction = 0
            };
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sal);
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sal);
            _mockMapper.Setup(m => m.Map<SalaryResponseDto>(sal)).Returns(new SalaryResponseDto());

            await _service.UpdateSalaryAsync(1, new SalaryUpdateDto { Bonus = 5000 }, "admin");

            Assert.Equal(5000, sal.Bonus);
            Assert.Equal("Corrected", sal.PayslipStatus);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetYearlySalaryReport_NoRecords_ThrowsNotFound()
        {
            _mockSalaryRepo.Setup(r => r.GetYearlyAllAsync(2025)).ReturnsAsync(new List<Salary>());
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetYearlySalaryReportAsync(2025));
        }

        // Update Recalculates Totals Correctly

        [Fact]
        public async Task UpdateSalary_Recalculates_GrossAndNetCorrectly()
        {
            var sal = new Salary
            {
                Id = 1, Basic = 20000, HRA = 10000, DA = 2000,
                TravelAllowance = 1600, SpecialAllowance = 5000, Bonus = 0,
                EmployeePF = 2400, ProfessionalTax = 200, IncomeTax = 1000, LopDeduction = 0
            };
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sal);
            _mockMapper.Setup(m => m.Map<SalaryResponseDto>(sal)).Returns(new SalaryResponseDto());

            await _service.UpdateSalaryAsync(1, new SalaryUpdateDto { Bonus = 5000, IncomeTax = 2000 }, "hr_admin");

            Assert.Equal(5000, sal.Bonus);
            Assert.Equal(2000, sal.IncomeTax);
            Assert.Equal(20000 + 10000 + 2000 + 1600 + 5000 + 5000, sal.GrossEarnings);
            Assert.Equal(2400 + 200 + 2000 + 0, sal.TotalDeductions);
            Assert.Equal(sal.GrossEarnings - sal.TotalDeductions, sal.NetSalary);
            Assert.Equal("Corrected", sal.PayslipStatus);
            Assert.Equal("hr_admin", sal.UpdatedBy);
        }

        // Negative Net Salary Edge Case

        [Fact]
        public async Task UpdateSalary_DeductionsExceedGross_NetBecomesZero()
        {
            var sal = new Salary
            {
                Id = 1, Basic = 5000, HRA = 2000, DA = 500,
                TravelAllowance = 500, SpecialAllowance = 0, Bonus = 0,
                EmployeePF = 600, ProfessionalTax = 0, IncomeTax = 0, LopDeduction = 0
            };
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sal);
            _mockMapper.Setup(m => m.Map<SalaryResponseDto>(sal)).Returns(new SalaryResponseDto());

            await _service.UpdateSalaryAsync(1, new SalaryUpdateDto { LopDeduction = 50000 }, "admin");

            Assert.Equal(0, sal.NetSalary);
        }

        // Update Sets Audit Fields

        [Fact]
        public async Task UpdateSalary_SetsUpdatedAtAndUpdatedBy()
        {
            var sal = new Salary
            {
                Id = 1, Basic = 20000, HRA = 10000, DA = 2000,
                TravelAllowance = 1600, SpecialAllowance = 5000, Bonus = 0,
                EmployeePF = 2400, ProfessionalTax = 200, IncomeTax = 1000, LopDeduction = 0,
                UpdatedAt = null, UpdatedBy = null
            };
            _mockSalaryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sal);
            _mockMapper.Setup(m => m.Map<SalaryResponseDto>(sal)).Returns(new SalaryResponseDto());

            await _service.UpdateSalaryAsync(1, new SalaryUpdateDto { Bonus = 1000 }, "payroll_admin");

            Assert.NotNull(sal.UpdatedAt);
            Assert.Equal("payroll_admin", sal.UpdatedBy);
            Assert.True((DateTime.Now - sal.UpdatedAt.Value).TotalSeconds < 5);
        }

        // All Employees Already Generated

        [Fact]
        public async Task GenerateSalary_AllAlreadyGenerated_ThrowsBadRequest()
        {
            var emps = new List<Employee> { new Employee { Id = 1, IsActive = true, AnnualCTC = 600000 } };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);
            _mockStructRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<SalaryStructure> { new SalaryStructure() });
            _mockSalaryRepo.Setup(r => r.ExistsAsync(1, 1, 2025)).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.GenerateMonthlySalaryAsync(1, 2025, "admin"));
        }
    }
}


