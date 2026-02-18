using EmpMS.DTOs.Auth;

namespace EmpMS.Services
{
    public interface IRolePrivilegeService
    {
        Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto);
        Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId);
        Task<List<PrivilegeDto>> GetPrivilegesByRoleIdAsync(int roleId);
    }
}
