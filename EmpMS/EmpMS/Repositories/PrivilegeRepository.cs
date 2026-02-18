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

        public async Task<Privilege> GetPrivilegeByIdAsync(int id)
        {
            return await _appDbContext.Privileges.FindAsync(id);
        }

        public async Task<bool> PrivilegeExistsAsync(string name)
        {
            return await _appDbContext.Privileges.AnyAsync(p => p.PrivilegeName == name);
        }

        public async Task UpdatePrivilegeAsync(Privilege privilege)
        {
            _appDbContext.Privileges.Update(privilege);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeletePrivilegeAsync(Privilege privilege)
        {
            _appDbContext.Privileges.Remove(privilege);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
