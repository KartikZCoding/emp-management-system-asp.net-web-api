using Application.DTOs.Leave;
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
    public class LeaveService : ILeaveService
    {

        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveService> _logger;
        private readonly IEmailService _emailService;

        public LeaveService(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository, IMapper mapper, ILogger<LeaveService> logger, IEmailService emailService)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<List<LeaveTypeResponseDto>> GetAllLeaveTypesAsync()
        {
            var leaves = await _leaveRepository.GetAllLeaveTypesAsync();
            if (leaves.Count == 0) throw new NotFoundException("Leaves not found!");

            return _mapper.Map<List<LeaveTypeResponseDto>>(leaves);
        }

        public async Task<LeaveTypeResponseDto> GetLeaveTypeByIdAsync(int id)
        {
            var leave = await _leaveRepository.GetLeaveTypeByIdAsync(id);
            if (leave == null) throw new NotFoundException($"Not found any leave for that {id}-Id!");

            return _mapper.Map<LeaveTypeResponseDto>(leave);
        }

        public async Task<LeaveTypeResponseDto> CreateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var existLeave = await _leaveRepository.LeaveTypeExistsAsync(dto.Name);
            if (existLeave) throw new BadRequestException("Leave name already exists!");

            var leaveType = new LeaveType
            {
                Name = dto.Name,
                Description = dto.Description,
                DefaultDays = dto.DefaultDays,
                IsPaid = dto.IsPaid,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            await _leaveRepository.CreateLeaveTypeAsync(leaveType);

            return _mapper.Map<LeaveTypeResponseDto>(leaveType);
        }

        public async Task<LeaveTypeResponseDto> UpdateLeaveTypeAsync(int id, LeaveTypeDto dto)
        {
            var existingLeave = await _leaveRepository.GetLeaveTypeByIdAsync(id);
            if (existingLeave == null) throw new NotFoundException("Leave type not found!");

            _mapper.Map(dto, existingLeave);
            existingLeave.UpdatedAt = DateTime.Now;

            await _leaveRepository.UpdateLeaveTypeAsync(existingLeave);
            return _mapper.Map<LeaveTypeResponseDto>(existingLeave);
        }

        public async Task DeleteLeaveTypeAsync(int id)
        {
            var existingLeave = await _leaveRepository.GetLeaveTypeByIdAsync(id);
            if (existingLeave == null) throw new NotFoundException("Leave type not found!");

            existingLeave.IsActive = false;
            existingLeave.UpdatedAt = DateTime.Now;

            await _leaveRepository.UpdateLeaveTypeAsync(existingLeave);
        }

        public async Task<List<LeaveBalanceResponseDto>> GetMyBalancesAsync(string employeeEmail, int year)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee not found!");

            var balance = await _leaveRepository.GetBalancesByEmployeeAsync(employee.Id, year);
            if (balance.Count == 0) throw new NotFoundException("No leave balances found for this year!");

            return _mapper.Map<List<LeaveBalanceResponseDto>>(balance);
        }

        public async Task AssignBalancesAsync(int employeeId, int year)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) throw new NotFoundException($"Employee with ID {employeeId} not found!");

            var existingBalances = await _leaveRepository.GetBalancesByEmployeeAsync(employeeId, year);
            if (existingBalances.Count > 0) throw new BadRequestException($"Leave balances already assigned for year {year}!");

            await _leaveRepository.AssignBalancesForEmployeeAsync(employeeId, year);
        }

        public async Task<LeaveRequestResponseDto> ApplyLeaveAsync(string employeeEmail, LeaveRequestDto dto)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))
                throw new BadRequestException("Cannot apply leave for past dates!");

            if (dto.EndDate < dto.StartDate)
                throw new BadRequestException("End date must be on or after start date!");

            var leaveType = await _leaveRepository.GetLeaveTypeByIdAsync(dto.LeaveTypeId);
            if (leaveType == null) throw new NotFoundException("Leave type not found!");

            var hasOverlap = await _leaveRepository.HasOverlappingRequestAsync(employee.Id, dto.StartDate, dto.EndDate);
            if (hasOverlap) throw new BadRequestException("You already have a leave request overlapping these dates!");

            int totalDays = (dto.EndDate.DayNumber - dto.StartDate.DayNumber) + 1;

            var balance = await _leaveRepository.GetBalanceAsync(employee.Id, dto.LeaveTypeId, dto.StartDate.Year);
            if (balance == null) throw new BadRequestException("No leave balance found! Ask HR to assign balances.");
            if (balance.RemainingLeaves < totalDays)
                throw new BadRequestException($"Insufficient leave balance! You have {balance.RemainingLeaves} days remaining but requested {totalDays} days.");

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employee.Id,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalDays = totalDays,
                Reason = dto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            await _leaveRepository.CreateRequestAsync(leaveRequest);

            var created = await _leaveRepository.GetRequestByIdAsync(leaveRequest.Id);
            return _mapper.Map<LeaveRequestResponseDto>(created);
        }

        public async Task<LeaveRequestResponseDto> ApproveLeaveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            var request = await _leaveRepository.GetRequestByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Leave request not found!");

            if (request.Status != "Pending")
                throw new BadRequestException($"Request is already {request.Status}!");

            var balance = await _leaveRepository.GetBalanceAsync(request.EmployeeId, request.LeaveTypeId, request.StartDate.Year);
            if (balance == null) throw new BadRequestException("Leave balance not found!");
            if (balance.RemainingLeaves < request.TotalDays)
                throw new BadRequestException($"Insufficient balance! Employee has {balance.RemainingLeaves} days remaining but needs {request.TotalDays}.");

            request.Status = "Approved";
            request.ApprovedById = hrUserId;
            request.DecisionDate = DateTime.Now;
            request.DecisionNote = decisionNote;
            request.UpdatedAt = DateTime.Now;

            await _leaveRepository.UpdateRequestAsync(request);

            balance.UsedLeaves += request.TotalDays;
            balance.RemainingLeaves -= request.TotalDays;

            await _leaveRepository.UpdateBalanceAsync(balance);

            var employeeName = request.Employee.FirstName + " " + request.Employee.LastName;
            await _emailService.SendEmailAsync(
                request.Employee.Email,
                "Leave Approved ✅",
                $"Dear {employeeName},\n\n" +
                $"Your {request.LeaveType.Name} request from {request.StartDate:dd-MMM-yyyy} to {request.EndDate:dd-MMM-yyyy} ({request.TotalDays} days) has been APPROVED.\n\n" +
                $"HR Note: {decisionNote ?? "N/A"}\n\n" +
                $"Regards,\nEmpMS HR Team"
            );

            return _mapper.Map<LeaveRequestResponseDto>(request);
        }

        public async Task<LeaveRequestResponseDto> RejectLeaveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            var request = await _leaveRepository.GetRequestByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Leave request not found!");

            if (request.Status != "Pending")
                throw new BadRequestException($"Request is already {request.Status}!");

            request.Status = "Rejected";
            request.ApprovedById = hrUserId;
            request.DecisionDate = DateTime.Now;
            request.DecisionNote = decisionNote;
            request.UpdatedAt = DateTime.Now;

            await _leaveRepository.UpdateRequestAsync(request);

            var employeeName = request.Employee.FirstName + " " + request.Employee.LastName;
            await _emailService.SendEmailAsync(
                request.Employee.Email,
                "Leave Rejected ❌",
                $"Dear {employeeName},\n\n" +
                $"Your {request.LeaveType.Name} request from {request.StartDate:dd-MMM-yyyy} to {request.EndDate:dd-MMM-yyyy} ({request.TotalDays} days) has been REJECTED.\n\n" +
                $"Reason: {decisionNote ?? "No reason provided"}\n\n" +
                $"If you have questions, please contact HR.\n\n" +
                $"Regards,\nEmpMS HR Team"
            );

            return _mapper.Map<LeaveRequestResponseDto>(request);
        }

        public async Task<LeaveRequestResponseDto> CancelLeaveAsync(string employeeEmail, int requestId)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var request = await _leaveRepository.GetRequestByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Leave request not found!");

            if (request.EmployeeId != employee.Id)
                throw new BadRequestException("You can only cancel your own requests!");

            if (request.Status == "Rejected")
                throw new BadRequestException("Cannot cancel an already rejected request!");
            if (request.Status == "Cancelled")
                throw new BadRequestException("Request is already cancelled!");

            if (request.Status == "Approved")
            {
                var balance = await _leaveRepository.GetBalanceAsync(employee.Id, request.LeaveTypeId, request.StartDate.Year);
                if (balance != null)
                {
                    balance.UsedLeaves -= request.TotalDays;
                    balance.RemainingLeaves += request.TotalDays;
                    await _leaveRepository.UpdateBalanceAsync(balance);
                }
            }

            request.Status = "Cancelled";
            request.UpdatedAt = DateTime.Now;

            await _leaveRepository.UpdateRequestAsync(request);

            return _mapper.Map<LeaveRequestResponseDto>(request);
        }

        public async Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(string employeeEmail)
        {
            var employee = await _employeeRepository.GetByEmailAsync(employeeEmail);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var requests = await _leaveRepository.GetRequestsByEmployeeAsync(employee.Id);
            if (requests.Count == 0) throw new NotFoundException("No leave requests found!");

            return _mapper.Map<List<LeaveRequestResponseDto>>(requests);
        }

        public async Task<List<LeaveRequestResponseDto>> GetPendingRequestAsync()
        {
            var requests = await _leaveRepository.GetPendingRequestAsync();
            if (requests.Count == 0) throw new NotFoundException("No pending leave requests found!");

            return _mapper.Map<List<LeaveRequestResponseDto>>(requests);
        }

    }
}
