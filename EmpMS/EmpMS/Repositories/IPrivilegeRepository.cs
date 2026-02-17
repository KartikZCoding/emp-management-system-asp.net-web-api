using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IPrivilegeRepository
    {
        Task<List<Privilege>> GetAllPrivilegesAsync();
        Task CreatePrivilegeAsync(Privilege privilege);
    }
}
