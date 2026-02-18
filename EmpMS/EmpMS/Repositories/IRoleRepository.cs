using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task CreateRoleAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(int id);
    }
}
