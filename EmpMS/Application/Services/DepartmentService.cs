using AutoMapper;
using Application.DTOs.Department;
using Application.DTOs.Employee;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<List<DepartmentResponseDto>>(departments);
        }

        public async Task<DepartmentResponseDto> GetDepartmentByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException("Department not found!");

            return _mapper.Map<DepartmentResponseDto>(department);
        }

        public async Task CreateDepartmentAsync(DepartmentDto departmentDto)
        {
            if (string.IsNullOrWhiteSpace(departmentDto.DepartmentName))
                throw new BadRequestException("Please enter a department name!");

            if (await _departmentRepository.ExistsAsync(departmentDto.DepartmentName))
                throw new BadRequestException("Department name already exists!");

            var department = _mapper.Map<Department>(departmentDto);
            department.CreatedAt = DateTime.Now;

            await _departmentRepository.CreateAsync(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(int id, DepartmentDto departmentDto)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            if (string.IsNullOrWhiteSpace(departmentDto.DepartmentName))
                throw new BadRequestException("Please enter a department name!");

            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException("Department not found!");

            _mapper.Map(departmentDto, department);
            department.UpdatedAt = DateTime.Now;

            await _departmentRepository.UpdateAsync(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException("Department not found!");

            //soft delete
            department.IsActive = false;
            department.UpdatedAt = DateTime.Now;

            await _departmentRepository.DeleteAsync(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EmployeeListDto>> GetEmployeesInDepartmentAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new BadRequestException("Enter a valid department id!");

            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
                throw new NotFoundException("Department not found!");

            var employees = await _departmentRepository.GetEmployeesByDepartmentIdAsync(departmentId);
            return _mapper.Map<List<EmployeeListDto>>(employees);
        }
    }
}
