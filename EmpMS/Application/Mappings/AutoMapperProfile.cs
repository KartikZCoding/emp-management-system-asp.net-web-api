using Application.DTOs.Attendance;
using Application.DTOs.Auth;
using Application.DTOs.Department;
using Application.DTOs.Designation;
using Application.DTOs.Employee;
using Application.DTOs.Leave;
using Application.DTOs.Salary;
using Application.DTOs.Review;
using Application.DTOs.Notification;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RoleDto, Role>().ReverseMap();

            //Department mappings
            CreateMap<DepartmentDto, Department>().ReverseMap();
            CreateMap<Department, DepartmentResponseDto>();

            //Designation mappings
            CreateMap<DesignationDto, Designation>().ReverseMap();
            CreateMap<Designation, DesignationResponseDto>();

            //Employee -> CreateEmployeeDto
            CreateMap<CreateEmployeeDto, Employee>().ReverseMap();

            //Employee -> UpdateEmployeeDto
            CreateMap<UpdateEmployeeDto, Employee>().ReverseMap();

            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(src => src.Designation.DesignationName))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FirstName + " " + src.Manager.LastName : null));

            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(src => src.Designation.DesignationName));

            //UpdateOwnProfileDto -> Employee
            CreateMap<UpdateOwnProfileDto, Employee>().ReverseMap();

            //Attendance mappings
            CreateMap<AttendanceLog, AttendanceLogResponseDto>();

            CreateMap<Attendance, AttendanceResponseDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.Logs, opt => opt.MapFrom(src => src.AttendanceLogs));

            // Attendance Regularization mappings
            CreateMap<AttendanceRegularization, AttendanceRegularizationResponseDto>();

            // Leave Type mappings
            CreateMap<LeaveTypeDto, LeaveType>().ReverseMap();
            CreateMap<LeaveType, LeaveTypeResponseDto>();

            // Leave Balance mappings
            CreateMap<LeaveBalance, LeaveBalanceResponseDto>()
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name));

            // Leave Request mappings
            CreateMap<LeaveRequest, LeaveRequestResponseDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name));

            // Salary mappings
            CreateMap<Salary, SalaryResponseDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(
                    src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(
                    src => src.Employee.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(
                    src => src.Employee.Designation.DesignationName))
                .ForMember(dest => dest.MonthName, opt => opt.MapFrom(
                    src => new DateTime(src.Year, src.Month, 1).ToString("MMMM yyyy")));

            // Performance Review mappings
            CreateMap<PerformanceReview, ReviewResponseDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(
                    src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(
                    src => src.Employee.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(
                    src => src.Employee.Designation.DesignationName))
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(
                    src => src.Reviewer.FirstName + " " + src.Reviewer.LastName))
                .ForMember(dest => dest.RatingLabel, opt => opt.Ignore()); // Set manually in service

            // Notification mappings
            CreateMap<Notification, NotificationResponseDto>();

        }
    }
}
