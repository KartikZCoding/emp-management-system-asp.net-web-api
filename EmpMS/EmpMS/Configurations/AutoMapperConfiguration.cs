using AutoMapper;
using EmpMS.DTOs.Auth;
using EmpMS.Models;

namespace EmpMS.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
