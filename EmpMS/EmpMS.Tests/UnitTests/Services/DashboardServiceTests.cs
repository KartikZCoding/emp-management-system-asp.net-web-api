using Application.DTOs.Dashboard;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IDepartmentRepository> _mockDeptRepo;
        private readonly Mock<IDesignationRepository> _mockDesigRepo;
        private readonly Mock<IAttendanceRepository> _mockAttRepo;
        private readonly Mock<ILeaveRepository> _mockLeaveRepo;
        private readonly Mock<ISalaryRepository> _mockSalaryRepo;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockDeptRepo = new Mock<IDepartmentRepository>();
            _mockDesigRepo = new Mock<IDesignationRepository>();
            _mockAttRepo = new Mock<IAttendanceRepository>();
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockSalaryRepo = new Mock<ISalaryRepository>();
            _service = new DashboardService(
                _mockEmpRepo.Object, _mockDeptRepo.Object, _mockDesigRepo.Object,
                _mockAttRepo.Object, _mockLeaveRepo.Object, _mockSalaryRepo.Object);
        }

        [Fact]
        public async Task GetSummary_ReturnsCorrectCounts()
        {
            var emps = new List<Employee>
            {
                new Employee { Id = 1, IsActive = true },
                new Employee { Id = 2, IsActive = true },
                new Employee { Id = 3, IsActive = false }
            };
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, 100000, null, null)).ReturnsAsync(emps);
            _mockAttRepo.Setup(r => r.GetTodayAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Attendance>());
            _mockLeaveRepo.Setup(r => r.GetAllRequestsAsync()).ReturnsAsync(new List<LeaveRequest>());
            _mockDeptRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Department> { new Department() });
            _mockDesigRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Designation> { new Designation(), new Designation() });
            _mockSalaryRepo.Setup(r => r.GetAllByMonthYearAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<Salary>());

            var result = await _service.GetSummaryAsync();

            Assert.Equal(3, result.TotalEmployees);
            Assert.Equal(2, result.ActiveEmployees);
            Assert.Equal(1, result.InactiveEmployees);
            Assert.Equal(1, result.TotalDepartments);
            Assert.Equal(2, result.TotalDesignations);
        }

        [Fact]
        public async Task GetSummary_WithPendingLeaves_CountsCorrectly()
        {
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, 100000, null, null)).ReturnsAsync(new List<Employee>());
            _mockAttRepo.Setup(r => r.GetTodayAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Attendance>());
            _mockLeaveRepo.Setup(r => r.GetAllRequestsAsync()).ReturnsAsync(new List<LeaveRequest>
            {
                new LeaveRequest { Status = "Pending" },
                new LeaveRequest { Status = "Pending" },
                new LeaveRequest { Status = "Approved" }
            });
            _mockDeptRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Department>());
            _mockDesigRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Designation>());
            _mockSalaryRepo.Setup(r => r.GetAllByMonthYearAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<Salary>());

            var result = await _service.GetSummaryAsync();

            Assert.Equal(2, result.PendingLeaveRequests);
        }

        [Fact]
        public async Task GetDepartmentStats_CalculatesPercentages()
        {
            var depts = new List<Department>
            {
                new Department { Id = 1, DepartmentName = "IT" },
                new Department { Id = 2, DepartmentName = "HR" }
            };
            var emps = new List<Employee>
            {
                new Employee { Id = 1, DepartmentId = 1, IsActive = true },
                new Employee { Id = 2, DepartmentId = 1, IsActive = true },
                new Employee { Id = 3, DepartmentId = 2, IsActive = true },
                new Employee { Id = 4, DepartmentId = 2, IsActive = false }
            };
            _mockDeptRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(depts);
            _mockEmpRepo.Setup(r => r.GetAllAsync(1, 100000, null, null)).ReturnsAsync(emps);

            var result = await _service.GetDepartmentStatsAsync();

            Assert.Equal(3, result.TotalActiveEmployees);
            Assert.Equal(2, result.Departments.Count);
        }

        [Fact]
        public async Task GetLeaveStats_CalculatesBreakdown()
        {
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { Status = "Approved", StartDate = new DateOnly(2025, 3, 1), EndDate = new DateOnly(2025, 3, 3), LeaveType = new LeaveType { Name = "Sick" } },
                new LeaveRequest { Status = "Pending", StartDate = new DateOnly(2025, 6, 1), EndDate = new DateOnly(2025, 6, 2), LeaveType = new LeaveType { Name = "Casual" } }
            };
            _mockLeaveRepo.Setup(r => r.GetAllRequestsAsync()).ReturnsAsync(leaveRequests);

            var result = await _service.GetLeaveStatsAsync(2025);

            Assert.Equal(2025, result.Year);
            Assert.Equal(2, result.TotalLeaveRequests);
            Assert.Equal(1, result.Approved);
            Assert.Equal(1, result.Pending);
        }

        [Fact]
        public async Task GetSalaryStats_CalculatesAggregates()
        {
            var salaries = new List<Salary>
            {
                new Salary { NetSalary = 50000, Employee = new Employee { Department = new Department { DepartmentName = "IT" } } },
                new Salary { NetSalary = 60000, Employee = new Employee { Department = new Department { DepartmentName = "IT" } } }
            };
            _mockSalaryRepo.Setup(r => r.GetYearlyAllAsync(2025)).ReturnsAsync(salaries);

            var result = await _service.GetSalaryStatsAsync(2025);

            Assert.Equal(2025, result.Year);
            Assert.Equal(110000, result.TotalAnnualExpenditure);
            Assert.Equal(55000, result.AverageMonthlySalary);
            Assert.Equal(60000, result.HighestSalary);
            Assert.Equal(50000, result.LowestSalary);
        }

        [Fact]
        public async Task GetSalaryStats_NoSalaries_ReturnsZeros()
        {
            _mockSalaryRepo.Setup(r => r.GetYearlyAllAsync(2025)).ReturnsAsync(new List<Salary>());

            var result = await _service.GetSalaryStatsAsync(2025);

            Assert.Equal(0, result.TotalAnnualExpenditure);
            Assert.Equal(0, result.AverageMonthlySalary);
            Assert.Equal(0, result.HighestSalary);
            Assert.Equal(0, result.LowestSalary);
        }
    }
}


