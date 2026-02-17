using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IRolePrivilegeRepository
    {
        Task AssignPrivilegeToRoleAsync(RolePrivilege rolePrivilege);
        Task RemoveRolePrivilegeAsync(int id);
        Task<List<RolePrivilege>> GetPrivilegesByRoleIdAsync(int roleId);
    }
}
