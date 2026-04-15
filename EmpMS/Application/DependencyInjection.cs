using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            /* register services */
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPrivilegeService, PrivilegeService>();
            services.AddScoped<IRolePrivilegeService, RolePrivilegeService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IAttendanceRegularizationService, AttendanceRegularizationService>();
            services.AddScoped<ILeaveService, LeaveService>();

            /* register automapper 13+ syntax */
            services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile));


            return services;
        }
    }
}
