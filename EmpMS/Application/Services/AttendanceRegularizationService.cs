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
    public class AttendanceRegularizationService : IAttendanceRegularizationService
    {

        private readonly IAttendanceRegularizationRepository _attendanceRegularizationRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AttendanceRegularizationService(IAttendanceRegularizationRepository attendanceRegularizationRepository, IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IMapper mapper, ILogger logger)
        {
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AttendanceRegularizationResponseDto> CreateRequestAsync(string employeeEmail, AttendanceRegularizationRequestDto dto)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var attendance = await _attendanceRepository.GetByIdAsync(dto.AttendanceId);
            if (attendance == null) throw new NotFoundException("Attendance record not found!");

            if(attendance.EmployeeId != employee.Id) throw new BadRequestException("This attendance record does not belong to you!");

            var openLog = attendance.AttendanceLogs.FirstOrDefault(l => l.CheckOut == null);
            if (openLog == null) throw new BadRequestException("This attendance record has no missing checkout!");

            var hasPending = await _attendanceRegularizationRepository.HasPendingRequestAsync(dto.AttendanceId);
            if (hasPending) throw new BadRequestException("A pending regularization request already exists for this attendance!");

            var regularization = new AttendanceRegularization
            {
                EmployeeId = employee.Id,
                AttendanceId = dto.AttendanceId,
                Date = dto.Date,
                RequestedCheckOut = dto.RequestedCheckOut,
                Note = dto.Note,
                Status = "Pending",      // always starts as Pending
                HRorAdminId = null,      // no decision yet
                DecisionDate = null,
                DecisionNote = null,
                CreatedAt = DateTime.Now
            };

            await _attendanceRegularizationRepository.CreateAsync(regularization);
            return _mapper.Map<AttendanceRegularizationResponseDto>(regularization);


        }

        public Task<List<AttendanceRegularizationResponseDto>> GetMyRequestsAsync(string employeeEmail)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceRegularizationResponseDto>> GetPendingRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceRegularizationResponseDto> ApproveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceRegularizationResponseDto> RejectAsync(int requestId, int hrUserId, string? decisionNote)
        {
            throw new NotImplementedException();
        }
    }
}
