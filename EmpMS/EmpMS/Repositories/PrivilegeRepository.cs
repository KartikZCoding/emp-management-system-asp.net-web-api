using EmpMS.Data;
using EmpMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMS.Repositories
{
    public class PrivilegeRepository : IPrivilegeRepository
    {
        private readonly AppDbContext _appDbContext;

        public PrivilegeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task CreatePrivilegeAsync(Privilege privilege)
        {
            await _appDbContext.Privileges.AddAsync(privilege);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<Privilege>> GetAllPrivilegesAsync()
        {
            return await _appDbContext.Privileges.ToListAsync();
        }
    }
}
