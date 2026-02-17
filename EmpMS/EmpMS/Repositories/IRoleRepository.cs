using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(int id);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
