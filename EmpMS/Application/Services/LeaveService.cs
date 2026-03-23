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

        public LeaveService(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepositor, IMapper mapper, ILogger<LeaveService> logger, IEmailService emailService)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepositor;
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

        public Task<List<LeaveBalanceResponseDto>> GetMyBalancesAsync(string employeeEmail, int year)
        {
            
        }


        public Task<LeaveRequestResponseDto> ApplyLeaveAsync(string employeeEmail, LeaveRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<LeaveRequestResponseDto> ApproveLeaveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            throw new NotImplementedException();
        }

        public Task AssignBalancesAsync(int employeeId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<LeaveRequestResponseDto> CancelLeaveAsync(string employeeEmail, int requestId)
        {
            throw new NotImplementedException();
        }






        public Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(string employeeEmail)
        {
            throw new NotImplementedException();
        }

        public Task<List<LeaveRequestResponseDto>> GetPendingRequestAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LeaveRequestResponseDto> RejectLeaveAsync(int requestId, int hrUserId, string? decisionNote)
        {
            throw new NotImplementedException();
        }

    }
}
