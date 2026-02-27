using Application.Common;
using Application.DTOs.Employee;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<EmployeeListDto>> GetAllEmployeesAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var employees = await _employeeRepository.GetAllAsync(page, pageSize);
            if (employees == null) throw new BadRequestException("No record found");

            var totalCount = await _employeeRepository.GetTotalCountAsync();

            var employeeDtos = _mapper.Map<List<EmployeeListDto>>(employees);

            return new PaginatedResult<EmployeeListDto>
            {
                Items = employeeDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };

        }
        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Enter a valid employee ID!");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            return _mapper.Map<EmployeeResponseDto>(employee);
        }
        public async Task CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            if (await _employeeRepository.EmailExistAsync(dto.Email))
                throw new BadRequestException("An employee with this email already exists!");

            var employee = _mapper.Map<Employee>(dto);
            employee.CreatedAt = DateTime.Now;
            employee.IsActive = true;

            await _employeeRepository.CreateAsync(employee);
        }
        public async Task UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            if (id <= 0) throw new BadRequestException("Enter a valid employee ID!");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            if (employee.Email != dto.Email && await _employeeRepository.EmailExistAsync(dto.Email))
                throw new BadRequestException("This email already used by another employee!");

            _mapper.Map(dto, employee);

            employee.UpdatedAt = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee);

        }
        public async Task SoftDeleteEmployeeAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Enter a valid employee ID!");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            await _employeeRepository.SoftDeleteAsync(employee);
        }
        public async Task<List<EmployeeListDto>> SearchEmployeesAsync(string? name, int? deptId, int? designationId)
        {
            var employees = await _employeeRepository.SearchAsync(name, deptId, designationId);
            return _mapper.Map<List<EmployeeListDto>>(employees);
        }
        public async Task<List<EmployeeListDto>> GetByDepartmentAsync(int deptId)
        {
            var employees = await _employeeRepository.GetByDepartmentAsync(deptId);
            return _mapper.Map<List<EmployeeListDto>>(employees);
        }
        public async Task<List<EmployeeListDto>> GetByManagerAsync(int managerId)
        {
            var employees = await _employeeRepository.GetByManagerAsync(managerId);
            return _mapper.Map<List<EmployeeListDto>>(employees);
        }
        public async Task<EmployeeResponseDto> GetOwnProfileAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null)
                throw new NotFoundException("Employee profile not found!");

            return _mapper.Map<EmployeeResponseDto>(employee);
        }
        public async Task UpdateOwnProfileAsync(string email, UpdateOwnProfileDto dto)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null)
                throw new NotFoundException("Employee profile not found!");

            _mapper.Map(dto, employee);
            employee.UpdatedAt = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee);
        }

    }
}
