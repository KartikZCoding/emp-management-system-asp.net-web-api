using Application.Interfaces;
using Domain.Interfaces;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISalaryRepository _salaryRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public ReportService(
            IEmployeeRepository employeeRepository,
            ISalaryRepository salaryRepository,
            IAttendanceRepository attendanceRepository)
        {
            _employeeRepository = employeeRepository;
            _salaryRepository = salaryRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<byte[]> GenerateEmployeesReportCsvAsync()
        {
            var employees = await _employeeRepository.GetAllAsync(1, 100000, null, null);
            
            var sb = new StringBuilder();
            sb.AppendLine("ID,FirstName,LastName,Email,Phone,Department,Designation,JoiningDate,Status");

            foreach (var emp in employees)
            {
                var status = emp.IsActive ? "Active" : "Inactive";
                sb.AppendLine($"{emp.Id},{emp.FirstName},{emp.LastName},{emp.Email},{emp.Phone},{emp.Department?.DepartmentName},{emp.Designation?.DesignationName},{emp.JoinDate:yyyy-MM-dd},{status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateAttendanceReportCsvAsync(int month, int year)
        {
            var employees = await _employeeRepository.GetAllAsync(1, int.MaxValue, null, null);
            var attendance = await _attendanceRepository.GetMonthlyAllAsync(month, year);
            
            // Calculate working days (Mon-Fri)
            int workingDays = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var d = new DateTime(year, month, day);
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    workingDays++;
            }

            var sb = new StringBuilder();
            sb.AppendLine("EmployeeId,EmployeeName,Department,WorkingDays,PresentDays,AbsentDays,HalfDays,Leaves");

            foreach (var emp in employees)
            {
                var empAtt = attendance.Where(a => a.EmployeeId == emp.Id).ToList();
                int presentCount = empAtt.Count(a => a.Status == "Present");
                int halfDayCount = empAtt.Count(a => a.Status == "HalfDay");
                int leaveCount = empAtt.Count(a => a.Status == "OnLeave");
                
                // Effective present days considering half days
                decimal effectivePresent = presentCount + (halfDayCount * 0.5m);
                int absentCount = workingDays - (int)Math.Ceiling(effectivePresent) - leaveCount;
                if (absentCount < 0) absentCount = 0;

                sb.AppendLine($"{emp.Id},{emp.FirstName} {emp.LastName},{emp.Department?.DepartmentName},{workingDays},{presentCount},{absentCount},{halfDayCount},{leaveCount}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateSalaryReportCsvAsync(int month, int year)
        {
            var salaries = await _salaryRepository.GetAllByMonthYearAsync(month, year);
            
            var sb = new StringBuilder();
            sb.AppendLine("EmployeeId,EmployeeName,Basic,HRA,DA,GrossEarnings,PF,Tax,NetSalary");

            foreach(var sal in salaries)
            {
                sb.AppendLine($"{sal.EmployeeId},{sal.Employee?.FirstName} {sal.Employee?.LastName},{sal.Basic},{sal.HRA},{sal.DA},{sal.GrossEarnings},{sal.EmployeePF},{sal.IncomeTax},{sal.NetSalary}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
