using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateEmployeesReportCsvAsync();
        Task<byte[]> GenerateAttendanceReportCsvAsync(int month, int year);
        Task<byte[]> GenerateSalaryReportCsvAsync(int month, int year);
    }
}
