using Application.DTOs.Attendance;
using Application.DTOs.Auth;
using Application.DTOs.Department;
using Application.DTOs.Designation;
using Application.DTOs.Employee;
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

        }
    }
}
