using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IPrivilegeService
    {
        Task<List<PrivilegeDto>> GetAllPrivilegesAsync();
        Task<PrivilegeDto> GetPrivilegeByIdAsync(int id);
        Task CreatePrivilegeAsync(PrivilegeDto dto);
        Task UpdatePrivilegeAsync(int id, PrivilegeDto dto);
        Task DeletePrivilegeAsync(int id);
    }
}
