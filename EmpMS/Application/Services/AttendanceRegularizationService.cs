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
        private readonly ILogger<AttendanceRegularizationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceRegularizationService(
            IAttendanceRegularizationRepository attendanceRegularizationRepository,
            IAttendanceRepository attendanceRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            ILogger<AttendanceRegularizationService> logger,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AttendanceRegularizationResponseDto> CreateRequestAsync(string employeeEmail, AttendanceRegularizationRequestDto dto)
        {
            // 1. Find employee by email
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            // 2. Prevent regularization for today — use normal checkout instead!
            if (dto.Date >= DateOnly.FromDateTime(DateTime.Now))
                throw new BadRequestException("You cannot request regularization for today or future dates! Use the normal check-out.");

            // 3. Find attendance by employee + date (real-world: no AttendanceId needed)
            var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(employee.Id, dto.Date);
            if (attendance == null)
                throw new NotFoundException($"No attendance record found for {dto.Date}!");

            // 3. Check if there's actually a missing checkout
            var openLog = attendance.AttendanceLogs.FirstOrDefault(l => l.CheckOut == null);
            if (openLog == null) throw new BadRequestException("This attendance record has no missing checkout!");

            // 4. Prevent duplicate pending request
            var hasPending = await _attendanceRegularizationRepository.HasPendingRequestAsync(attendance.Id);
            if (hasPending) throw new BadRequestException("A pending regularization request already exists for this date!");

            // 5. Create entity — combine date + time for RequestedCheckOut
            var regularization = new AttendanceRegularization
            {
                EmployeeId = employee.Id,
                AttendanceId = attendance.Id,               // system fills this internally
                Date = attendance.Date.ToDateTime(TimeOnly.MinValue), // from attendance record
                RequestedCheckOut = attendance.Date.ToDateTime(dto.RequestedCheckOut), // date + time combined
                Note = dto.Note,
                Status = "Pending",
                HRorAdminId = null,
                DecisionDate = null,
                DecisionNote = null,
                CreatedAt = DateTime.Now
            };

            await _attendanceRegularizationRepository.CreateAsync(regularization);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AttendanceRegularizationResponseDto>(regularization);
        }

        public async Task<List<AttendanceRegularizationResponseDto>> GetMyRequestsAsync(string employeeEmail)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var requests = await _attendanceRegularizationRepository.GetByEmployeeIdAsync(employee.Id);
            if (requests.Count == 0) throw new NotFoundException("Not any regularization requests found!");

            return _mapper.Map<List<AttendanceRegularizationResponseDto>>(requests);
        }

        public async Task<List<AttendanceRegularizationResponseDto>> GetPendingRequestsAsync()
        {
            var requests = await _attendanceRegularizationRepository.GetPendingAsync();
            if (requests.Count == 0 || !requests.Any()) throw new NotFoundException("Does not found any pending request!");

            return _mapper.Map<List<AttendanceRegularizationResponseDto>>(requests);
        }

        public async Task<AttendanceRegularizationResponseDto> ApproveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            /*update regularization request*/
            var request = await _attendanceRegularizationRepository.GetByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Regularization request not found!");

            if (request.Status != "Pending") throw new BadRequestException($"Request is already {request.Status}!");

            request.Status = "Approved";
            request.HRorAdminId = hrUserId;
            request.DecisionDate = DateTime.Now;
            request.DecisionNote = decisionNote;

            await _attendanceRegularizationRepository.UpdateAsync(request);

            var attendance = request.Attendance;

            var openLog = attendance.AttendanceLogs.FirstOrDefault(a => a.CheckOut == null);
            if (openLog != null)
            {
                openLog.CheckOut = request.RequestedCheckOut;

                openLog.SessionHours = Math.Round((decimal)(openLog.CheckOut - openLog.CheckIn).Value.TotalHours, 2);

                attendance.TotalHours = attendance.AttendanceLogs.Where(l => l.SessionHours != null).Sum(l => l.SessionHours);

                attendance.IsCheckedIn = false;

                if (attendance.TotalHours < 5)
                    attendance.Status = "HalfDay";

                attendance.UpdatedAt = DateTime.Now;
                attendance.UpdatedBy = "Regularization-Approved";

                await _attendanceRepository.UpdateLogAsync(openLog);
                await _attendanceRepository.UpdateAsync(attendance);

            }

            // Send email notification to employee
            var employeeEmail = request.Employee.Email;
            var employeeName = request.Employee.FirstName + " " + request.Employee.LastName;
            await _emailService.SendEmailAsync(
                employeeEmail,
                "Attendance Regularization Approved ✅",
                $"Dear {employeeName},\n\n" +
                $"Your attendance regularization request for {request.Date:dd-MMM-yyyy} has been APPROVED.\n\n" +
                $"CheckOut Time Updated: {request.RequestedCheckOut:hh:mm tt}\n" +
                $"HR Note: {decisionNote ?? "N/A"}\n\n" +
                $"Regards,\nEmpMS HR Team"
            );
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AttendanceRegularizationResponseDto>(request);
        }

        public async Task<AttendanceRegularizationResponseDto> RejectAsync(int requestId, int hrUserId, string? decisionNote)
        {
            var request = await _attendanceRegularizationRepository.GetByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Regularization request not found!");

            if (request.Status != "Pending") throw new BadRequestException($"Request is already {request.Status}!");

            request.Status = "Rejected";
            request.HRorAdminId = hrUserId;
            request.DecisionDate = DateTime.Now;
            request.DecisionNote = decisionNote;

            await _attendanceRegularizationRepository.UpdateAsync(request);

            // Send email notification to employee
            var employeeEmail = request.Employee.Email;
            var employeeName = request.Employee.FirstName + " " + request.Employee.LastName;
            await _emailService.SendEmailAsync(
                employeeEmail,
                "Attendance Regularization Rejected ❌",
                $"Dear {employeeName},\n\n" +
                $"Your attendance regularization request for {request.Date:dd-MMM-yyyy} has been REJECTED.\n\n" +
                $"Reason: {decisionNote ?? "No reason provided"}\n\n" +
                $"If you have questions, please contact HR.\n\n" +
                $"Regards,\nEmpMS HR Team"
            );
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AttendanceRegularizationResponseDto>(request);
        }

        public async Task<List<AttendanceResponseDto>> GetMissedCheckoutsAsync(string employeeEmail)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var missed = await _attendanceRepository.GetMissedCheckoutsAsync(employee.Id);
            if (missed.Count == 0) throw new NotFoundException("No missed checkouts found!");

            return _mapper.Map<List<AttendanceResponseDto>>(missed);
        }
    }
}
