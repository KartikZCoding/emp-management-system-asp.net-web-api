using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IRolePrivilegeService
    {
        Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto);
        Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId);
        Task<List<PrivilegeResponseDto>> GetPrivilegesByRoleIdAsync(int roleId);
    }
}
