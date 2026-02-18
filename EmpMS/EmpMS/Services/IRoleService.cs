using EmpMS.DTOs.Auth;

namespace EmpMS.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(RoleDto roleDto);
        Task UpdateRoleAsync(int id, RoleDto roleDto);
        Task DeleteRoleAsync(int id);
    }
}
