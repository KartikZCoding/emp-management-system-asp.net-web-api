using Application.DTOs.Attendance;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IMapper mapper, ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<AttendanceResponseDto> CheckInAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(employee.Id, today);

            if (attendance != null)
            {
                var openLog = attendance.AttendanceLogs.FirstOrDefault(l => l.CheckOut == null);
                if (openLog != null) throw new BadRequestException("You are already checked in! check out first.");

                var newLog = new AttendanceLog
                {
                    AttendanceId = attendance.Id,
                    CheckIn = DateTime.Now,
                    CheckOut = null,
                    SessionHours = null,
                    CreatedAt = DateTime.Now
                };

                attendance.IsCheckedIn = true;
                attendance.Status = "Present";
                attendance.UpdatedAt = DateTime.Now;

                await _attendanceRepository.CreateLogAsync(newLog);
                await _attendanceRepository.UpdateAsync(attendance);

                return _mapper.Map<AttendanceResponseDto>(attendance);
            }

            var newAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Date = today,
                IsLate = DateTime.Now.TimeOfDay > new TimeSpan(10, 15, 0),
                Status = "Present",
                IsCheckedIn = true,
                TotalHours = null,
                CreatedAt = DateTime.Now
            };
            await _attendanceRepository.CreateAsync(newAttendance);

            var firstLog = new AttendanceLog
            {
                AttendanceId = newAttendance.Id,
                CheckIn = DateTime.Now,
                CheckOut = null,
                SessionHours = null,
                CreatedAt = DateTime.Now
            };

            await _attendanceRepository.CreateLogAsync(firstLog);

            return _mapper.Map<AttendanceResponseDto>(newAttendance);

        }

        public async Task<AttendanceResponseDto> CheckOutAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(employee.Id, today);
            if (attendance == null) throw new BadRequestException("You haven't checked in today!");

            var openLog = attendance.AttendanceLogs.FirstOrDefault(l => l.CheckOut == null);
            if (openLog == null) throw new BadRequestException("You are not currently checked in!");

            openLog.CheckOut = DateTime.Now;
            openLog.SessionHours = Math.Round((decimal)(openLog.CheckOut - openLog.CheckIn).Value.TotalHours, 2);

            attendance.TotalHours = attendance.AttendanceLogs.Where(l => l.SessionHours != null).Sum(l => l.SessionHours);
            attendance.IsCheckedIn = false;
            if (attendance.TotalHours < 5)
            {
                attendance.Status = "HalfDay";
            }
            attendance.UpdatedAt = DateTime.Now;

            await _attendanceRepository.UpdateAsync(attendance);

            return _mapper.Map<AttendanceResponseDto>(attendance);
        }

        public async Task<List<AttendanceResponseDto>> GetDepartmentAttendanceAsync(int deptId, DateOnly? date)
        {
            DateOnly targetDate = date ?? DateOnly.FromDateTime(DateTime.Now);
            var attendances = await _attendanceRepository.GetByDepartmentAndDateAsync(deptId, targetDate);

            return _mapper.Map<List<AttendanceResponseDto>>(attendances);
        }

        public async Task<List<AttendanceResponseDto>> GetEmployeeAttendanceAsync(int empId, int? month, int? year)
        {
            if (empId <= 0) throw new BadRequestException("Invalid employee id!");

            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            var attendaces = await _attendanceRepository.GetByEmployeeMonthlyAsync(empId, targetMonth, targetYear);

            return _mapper.Map<List<AttendanceResponseDto>>(attendaces);
        }

        public async Task<List<AttendanceReportDto>> GetMonthlyReportAsync(int? month, int? year)
        {
            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            var attendances = await _attendanceRepository.GetMonthlyAllAsync(targetMonth, targetYear);

            int totalDaysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);
            int workingDays = Enumerable.Range(1, totalDaysInMonth)
                .Count(day => new DateTime(targetYear, targetMonth, day).DayOfWeek != DayOfWeek.Saturday
                && new DateTime(targetYear, targetMonth, day).DayOfWeek != DayOfWeek.Sunday);

            var report = attendances
                .GroupBy(a => a.EmployeeId)
                .Select(group =>
                {
                    var records = group.ToList();
                    var employee = records.First().Employee;

                    int totalPresent = records.Count(a => a.Status == "Present");
                    int totalLate = records.Count(a => a.IsLate);
                    int totalHalfDays = records.Count(a => a.Status == "HalfDay");
                    int totalOnLeave = records.Count(a => a.Status == "OnLeave");

                    int totalAbsent = workingDays - (totalPresent + totalHalfDays + totalOnLeave);

                    decimal totalWorkHours = records.Sum(a => a.TotalHours ?? 0);
                    int workedDaysCount = records.Count(a => a.TotalHours > 0);
                    decimal averageWorkHours = workedDaysCount > 0
                                            ? Math.Round(totalWorkHours / workedDaysCount, 2)
                                            : 0;

                    return new AttendanceReportDto
                    {
                        EmployeeId = group.Key,
                        EmployeeName = employee.FirstName + " " + employee.LastName,
                        Month = targetMonth,
                        Year = targetYear,
                        TotalPresentDays = totalPresent,
                        TotalAbsentDays = totalAbsent < 0 ? 0 : totalAbsent,
                        TotalLateDays = totalLate,
                        TotalHalfDays = totalHalfDays,
                        TotalOnLeaveDays = totalOnLeave,
                        TotalWorkhours = totalWorkHours,
                        AverageWorkHours = averageWorkHours
                    };
                }).ToList();

            return report;

        }

        public async Task<List<AttendanceResponseDto>> GetMyAttendanceAsync(string email, int? month, int? year)
        {
            if (string.IsNullOrEmpty(email)) throw new BadRequestException("Invalid email");

            var employee = await _employeeRepository.GetByEmailAsync(email);

            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            var attendances = await _attendanceRepository.GetByEmployeeMonthlyAsync(employee.Id, targetMonth, targetYear);

            return _mapper.Map<List<AttendanceResponseDto>>(attendances);
        }

        public async Task<TodaySummaryDto> GetTodaySummaryAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var allToday = await _attendanceRepository.GetTodayAsync(today);

            var totalActiveEmployees = await _attendanceRepository.GetActiveEmployeeCountAsync();

            return new TodaySummaryDto
            {
                Date = today,
                TotalEmployees = totalActiveEmployees,
                TotalCheckedIn = allToday.Count,
                TotalPresent = allToday.Count(a => a.Status == "Present"),
                TotalLate = allToday.Count(a => a.IsLate),
                CurrentlyInOffice = allToday.Count(a => a.IsCheckedIn),
                TotalAbsent = totalActiveEmployees - allToday.Count
            };
        }

        public async Task<AttendanceResponseDto> UpdateAttendanceAsync(int id, AttendanceUpdateDto dto, string updateBy)
        {
            if (id <= 0) throw new BadRequestException("Invalid id!");

            var validStatuses = new[] { "Present", "HalfDay", "OnLeave" };
            if (!validStatuses.Contains(dto.Status))
                throw new BadRequestException("Invalid status! Use: Present, HalfDay, or OnLeave");

            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null) throw new NotFoundException("Attendance not found!");

            if (!string.IsNullOrEmpty(dto.Status))
            {
                attendance.Status = dto.Status;
            }
            attendance.UpdatedBy = updateBy;
            attendance.UpdatedAt = DateTime.Now;

            await _attendanceRepository.UpdateAsync(attendance);

            return _mapper.Map<AttendanceResponseDto>(attendance);
        }
    }
}
