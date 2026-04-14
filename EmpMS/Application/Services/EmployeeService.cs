using Application.Common;
using Application.DTOs.Employee;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, IWebHostEnvironment env, ILogger<EmployeeService> logger, IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<EmployeeListDto>> GetAllEmployeesAsync(int page, int pageSize, string? sortBy, string? sortOrder)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // normalize sortOrder to lowercase
            sortOrder = sortOrder?.ToLower() == "desc" ? "desc" : "asc";

            var employees = await _employeeRepository.GetAllAsync(page, pageSize, sortBy, sortOrder);
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
            await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task SoftDeleteEmployeeAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Enter a valid employee ID!");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            await _employeeRepository.SoftDeleteAsync(employee);
            await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UploadPhotoAsync(int id, IFormFile file)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            if (file == null || file.Length == 0)
                throw new BadHttpRequestException("No file uploaded!");

            var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
            var extention = Path.GetExtension(file.FileName).ToLower();
            //_logger.LogInformation($"Service : file extension is {extention}");

            if (!allowedExtentions.Contains(extention))
                throw new BadRequestException("Only .jpg, .jpeg, .png files are allowed!");

            if (file.Length > 2 * 1024 * 1024)
                throw new BadRequestException("File size must be less than 2MB!");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "photos");
            //_logger.LogInformation($"Service : upload folder is {uploadsFolder}");
            Directory.CreateDirectory(uploadsFolder);

            if(!string.IsNullOrEmpty(employee.PhotoPath))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, employee.PhotoPath.TrimStart('/'));
                //_logger.LogInformation($"Service : old file path is {oldFilePath}");
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            var fileName = $"employee_{id}{extention}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            //_logger.LogInformation($"Service : file name is {fileName}, file path is {filePath}");

            using (var strem = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(strem);
            }

            var relativePath = $"/uploads/photos/{fileName}";
            await _employeeRepository.UpdatePhotoPathAsync(id, relativePath);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<(byte[] fileBytes, string contentType)> GetPhotoAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found!");

            if (string.IsNullOrEmpty(employee.PhotoPath))
                throw new NotFoundException("No photo uploaded for this employee!");

            //_logger.LogInformation($"Service : get photo path is {employee.PhotoPath} without TrimStart.");
            var filePath = Path.Combine(_env.WebRootPath, employee.PhotoPath.TrimStart('/'));
            //_logger.LogInformation($"Service : get photo file path is {filePath} with TrimStart.");


            if (!File.Exists(filePath))
                throw new NotFoundException("Photo file not found on server!");

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var extension = Path.GetExtension(filePath).ToLower();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return (fileBytes, contentType);
        }
    }
}
