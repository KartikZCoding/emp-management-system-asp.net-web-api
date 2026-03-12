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

            var attendance =await _attendanceRepository.GetByEmployeeAndDateAsync(employee.Id, today);

            if(attendance != null)
            {
                var openLog = attendance.AttendanceLogs.FirstOrDefault(l => l.CheckOut == null);

                var newLog = new AttendanceLog
                {
                    AttendanceId = attendance.Id,
                    CheckIn = DateTime.Now,
                    CheckOut = null,
                    SessionHours = null,
                    CreatedAt = DateTime.Now
                };

                attendance.IsCheckedIn = true;
                attendance.UpdatedAt = DateTime.Now;

                await _attendanceRepository.CreateLogAsync(newLog);
                await _attendanceRepository.UpdateAsync(attendance);

                return _mapper.Map<AttendanceResponseDto>(attendance);
            }

            

        }

        public async Task<AttendanceResponseDto> CheckOutAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(employee.Id, today);
            if (attendance == null) throw new BadRequestException("You haven't checked in today!");

            var openLog = attendance.AttendanceLogs.FirstOrDefault(l =>l.CheckOut == null);
            if (openLog == null) throw new BadRequestException("You are not currently checked in!");

            openLog.CheckOut = DateTime.Now;
            openLog.SessionHours = Math.Round((decimal)(openLog.CheckOut - openLog.CheckIn).Value.TotalHours, 2);

            attendance.TotalHours = attendance.AttendanceLogs.Where(l => l.SessionHours != null).Sum(l => l.SessionHours);
            attendance.IsCheckedIn = false;
            if(attendance.TotalHours < 5)
            {
                attendance.Status = 
            }


        }

        public Task<List<AttendanceResponseDto>> GetDepartmentAttendanceAsync(int deptId, DateOnly? date)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceResponseDto>> GetEmployeeAttendanceAsync(int empId, int? month, int? year)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceReportDto>> GetMonthlyReportAsync(int? month, int? year)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceResponseDto>> GetMyAttendanceAsync(string email, int? month, int? year)
        {
            throw new NotImplementedException();
        }

        public Task<TodaySummaryDto> GetTodaySummaryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceResponseDto> UpdateAttendanceAsync(int id, AttendanceUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
