using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRolePrivilegeRepository
    {
        Task AddRolePrivilegeAsync(RolePrivilege rolePrivilege);
        Task DeleteRolePrivilegeAsync(RolePrivilege rolePrivilege);
        Task<List<Privilege>> GetPrivilegesByRoleIdAsync(int roleId);
        Task<RolePrivilege> GetRolePrivilegeAsync(int roleId, int privilegeId);
        Task<bool> RolePrivilegeExistsAsync(int roleId, int privilegeId);
    }
}
