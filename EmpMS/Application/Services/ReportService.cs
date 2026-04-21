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

        public ReportService(
            IEmployeeRepository employeeRepository,
            ISalaryRepository salaryRepository)
        {
            _employeeRepository = employeeRepository;
            _salaryRepository = salaryRepository;
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
            var sb = new StringBuilder();
            sb.AppendLine("EmployeeId,Month,Year,PresentDays,AbsentDays");
            // Placeholder logic for real attendance queries
            return await Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
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
