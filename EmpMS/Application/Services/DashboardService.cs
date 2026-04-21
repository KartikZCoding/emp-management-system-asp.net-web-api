using Application.DTOs.Dashboard;
using Application.Interfaces;
using Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDesignationRepository _designationRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ISalaryRepository _salaryRepository;

        public DashboardService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IDesignationRepository designationRepository,
            IAttendanceRepository attendanceRepository,
            ILeaveRepository leaveRepository,
            ISalaryRepository salaryRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _designationRepository = designationRepository;
            _attendanceRepository = attendanceRepository;
            _leaveRepository = leaveRepository;
            _salaryRepository = salaryRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var employees = await _employeeRepository.GetAllAsync(1, 100000, null, null);
            var activeEmployees = employees.Count(e => e.IsActive);
            
            var today = DateTime.Today;
            var attendancesToday = await _attendanceRepository.GetTodayAsync(DateOnly.FromDateTime(today));
            var leaves = await _leaveRepository.GetAllRequestsAsync();
            
            var currentMonthSalaries = await _salaryRepository.GetAllByMonthYearAsync(today.Month, today.Year);

            return new DashboardSummaryDto
            {
                TotalEmployees = employees.Count,
                ActiveEmployees = activeEmployees,
                InactiveEmployees = employees.Count - activeEmployees,
                TotalDepartments = (await _departmentRepository.GetAllAsync()).Count,
                TotalDesignations = (await _designationRepository.GetAllAsync()).Count,
                PendingLeaveRequests = leaves.Count(l => l.Status == "Pending"),
                TodayPresentCount = attendancesToday.Count(a => a.Status != "Absent"),
                TodayAbsentCount = attendancesToday.Count(a => a.Status == "Absent") + (activeEmployees - attendancesToday.Count), // Estimating absents
                AverageAttendancePercent = attendancesToday.Any() ? 
                    ((double)attendancesToday.Count(a => a.Status != "Absent") / activeEmployees) * 100 : 0,
                CurrentMonthSalaryExpenditure = currentMonthSalaries.Sum(s => s.NetSalary)
            };
        }

        public async Task<AttendanceOverviewDto> GetAttendanceOverviewAsync(int month, int year)
        {
            var activeEmployees = (await _employeeRepository.GetAllAsync(1, int.MaxValue, null, null))
                .Where(e => e.IsActive).ToList();
            int activeCount = activeEmployees.Count;

            var allAtt = await _attendanceRepository.GetMonthlyAllAsync(month, year);
            
            // Calculate actual working days (Mon-Fri)
            int totalWorkingDays = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalWorkingDays++;
                }
            }

            var dto = new AttendanceOverviewDto
            {
                Month = month,
                Year = year,
                TotalWorkingDays = totalWorkingDays,
                AveragePresentDays = activeCount > 0 ? (double)allAtt.Count(a => a.Status == "Present" || a.Status == "HalfDay") / activeCount : 0,
                AverageAbsentDays = activeCount > 0 ? (double)allAtt.Count(a => a.Status == "Absent") / activeCount : 0,
                LateCheckInCount = allAtt.Count(a => a.AttendanceLogs.Any(l => l.CheckIn.TimeOfDay > new TimeSpan(9, 30, 0))), // 9:30 AM policy
                MissedCheckoutCount = allAtt.Count(a => a.AttendanceLogs.Any(l => l.CheckOut == null)),
            };

            // Department-wise stats
            var deptGroups = allAtt.GroupBy(a => a.Employee.Department?.DepartmentName ?? "Unknown");
            foreach (var group in deptGroups)
            {
                int present = group.Count(a => a.Status == "Present" || a.Status == "HalfDay");
                int total = group.Count();

                dto.DepartmentWiseAttendance.Add(new DepartmentAttendanceDto
                {
                    DepartmentName = group.Key,
                    PresentCount = present,
                    AbsentCount = group.Count(a => a.Status == "Absent"),
                    PresentPercent = total > 0 ? (double)present / total * 100 : 0
                });
            }

            return dto;
        }

        public async Task<DepartmentStatsDto> GetDepartmentStatsAsync()
        {
            var depts = await _departmentRepository.GetAllAsync();
            var emps = await _employeeRepository.GetAllAsync(1, 100000, null, null);
            var activeEmps = emps.Where(e => e.IsActive).ToList();
            var totalActive = activeEmps.Count;

            var dto = new DepartmentStatsDto { TotalActiveEmployees = totalActive };

            foreach (var dept in depts)
            {
                var count = activeEmps.Count(e => e.DepartmentId == dept.Id);
                dto.Departments.Add(new DepartmentEmployeeCountDto
                {
                    DepartmentId = dept.Id,
                    DepartmentName = dept.DepartmentName,
                    EmployeeCount = count,
                    Percentage = totalActive > 0 ? ((double)count / totalActive) * 100 : 0
                });
            }

            return dto;
        }

        public async Task<LeaveStatsDto> GetLeaveStatsAsync(int year)
        {
            var allLeaves = await _leaveRepository.GetAllRequestsAsync();
            var yearLeaves = allLeaves.Where(l => l.StartDate.Year == year || l.EndDate.Year == year).ToList();
            var total = yearLeaves.Count;

            var dto = new LeaveStatsDto
            {
                Year = year,
                TotalLeaveRequests = total,
                Approved = yearLeaves.Count(l => l.Status == "Approved"),
                Rejected = yearLeaves.Count(l => l.Status == "Rejected"),
                Pending = yearLeaves.Count(l => l.Status == "Pending")
            };

            var groups = yearLeaves.GroupBy(l => l.LeaveType.Name);
            foreach (var group in groups)
            {
                dto.LeaveTypeBreakdown.Add(new LeaveTypeBreakdownDto
                {
                    LeaveType = group.Key,
                    Count = group.Count(),
                    Percentage = total > 0 ? ((double)group.Count() / total) * 100 : 0
                });
            }

            return dto;
        }

        public async Task<SalaryStatsDto> GetSalaryStatsAsync(int year)
        {
            var yearlySalaries = await _salaryRepository.GetYearlyAllAsync(year);
            
            var dto = new SalaryStatsDto
            {
                Year = year,
                TotalAnnualExpenditure = yearlySalaries.Sum(s => s.NetSalary),
                AverageMonthlySalary = yearlySalaries.Any() ? yearlySalaries.Average(s => s.NetSalary) : 0,
                HighestSalary = yearlySalaries.Any() ? yearlySalaries.Max(s => s.NetSalary) : 0,
                LowestSalary = yearlySalaries.Any() ? yearlySalaries.Min(s => s.NetSalary) : 0
            };

            // Department-wise stats
            var deptGroups = yearlySalaries.GroupBy(s => s.Employee.Department?.DepartmentName ?? "Unknown");
            foreach (var group in deptGroups)
            {
                decimal totalExpenditure = group.Sum(s => s.NetSalary);
                decimal avgSalary = group.Average(s => s.NetSalary);

                dto.DepartmentWiseSalary.Add(new DepartmentSalaryDto
                {
                    DepartmentName = group.Key,
                    TotalExpenditure = totalExpenditure,
                    AverageSalary = avgSalary,
                    TotalSalary = totalExpenditure, // compatibility with existing DTO property
                    AvgSalary = avgSalary           // compatibility with existing DTO property
                });
            }

            return dto;
        }
    }
}
