using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(RoleDto roleDto);
        Task UpdateRoleAsync(int id, RoleDto roleDto);
        Task DeleteRoleAsync(int id);
    }
}
