using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<ISalaryRepository> _mockSalaryRepo;
        private readonly Mock<IAttendanceRepository> _mockAttRepo;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockSalaryRepo = new Mock<ISalaryRepository>();
            _mockAttRepo = new Mock<IAttendanceRepository>();
            _service = new ReportService(_mockEmpRepo.Object, _mockSalaryRepo.Object, _mockAttRepo.Object);
        }

        [Fact]
        public async Task GenerateEmployeesReport_ReturnsCsvBytes()
        {
            var emps = new List<Employee>
            {
                new Employee
                {
                    Id = 1, FirstName = "John", LastName = "Doe",
                    Email = "j@t.com", Phone = "123", IsActive = true,
                    JoinDate = new DateTime(2024, 1, 1),
                    Department = new Department { DepartmentName = "IT" },
                    Designation = new Designation { DesignationName = "Dev" }
                }
            };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, 100000, null, null)).ReturnsAsync(emps);

            var result = await _service.GenerateEmployeesReportCsvAsync();

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("John", csv);
            Assert.Contains("IT", csv);
            Assert.Contains("Active", csv);
        }

        [Fact]
        public async Task GenerateEmployeesReport_EmptyList_ReturnsHeaderOnly()
        {
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, 100000, null, null)).ReturnsAsync(new List<Employee>());

            var result = await _service.GenerateEmployeesReportCsvAsync();

            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("ID,FirstName", csv);
        }

        [Fact]
        public async Task GenerateAttendanceReport_ReturnsCsvBytes()
        {
            var emps = new List<Employee>
            {
                new Employee
                {
                    Id = 1, FirstName = "John", LastName = "Doe",
                    Department = new Department { DepartmentName = "IT" }
                }
            };
            var attendances = new List<Attendance>
            {
                new Attendance { EmployeeId = 1, Status = "Present", AttendanceLogs = new List<AttendanceLog>() }
            };

            _mockEmpRepo.Setup(r => r.GetAllAsync(1, int.MaxValue, null, null)).ReturnsAsync(emps);
            _mockAttRepo.Setup(r => r.GetMonthlyAllAsync(1, 2025)).ReturnsAsync(attendances);

            var result = await _service.GenerateAttendanceReportCsvAsync(1, 2025);

            Assert.NotNull(result);
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("John Doe", csv);
        }

        [Fact]
        public async Task GenerateSalaryReport_ReturnsCsvBytes()
        {
            var salaries = new List<Salary>
            {
                new Salary
                {
                    EmployeeId = 1, Basic = 20000, HRA = 10000, DA = 2000,
                    GrossEarnings = 40000, EmployeePF = 2400, IncomeTax = 1000, NetSalary = 36000,
                    Employee = new Employee { FirstName = "John", LastName = "Doe" }
                }
            };
            _mockSalaryRepo.Setup(r => r.GetAllByMonthYearAsync(1, 2025)).ReturnsAsync(salaries);

            var result = await _service.GenerateSalaryReportCsvAsync(1, 2025);

            Assert.NotNull(result);
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("John Doe", csv);
            Assert.Contains("36000", csv);
        }

        [Fact]
        public async Task GenerateSalaryReport_NoSalaries_ReturnsHeaderOnly()
        {
            _mockSalaryRepo.Setup(r => r.GetAllByMonthYearAsync(1, 2025)).ReturnsAsync(new List<Salary>());

            var result = await _service.GenerateSalaryReportCsvAsync(1, 2025);

            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("EmployeeId,EmployeeName", csv);
        }
    }
}


