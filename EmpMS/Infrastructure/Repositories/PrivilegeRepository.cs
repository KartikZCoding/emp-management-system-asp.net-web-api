using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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

        }

        public async Task<List<Privilege>> GetAllPrivilegesAsync()
        {
            return await _appDbContext.Privileges.AsNoTracking().ToListAsync();
        }

        public async Task<Privilege> GetPrivilegeByIdAsync(int id)
        {
            return await _appDbContext.Privileges.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> PrivilegeExistsAsync(string name)
        {
            return await _appDbContext.Privileges.AnyAsync(p => p.PrivilegeName == name);
        }

        public async Task UpdatePrivilegeAsync(Privilege privilege)
        {
            _appDbContext.Privileges.Update(privilege);

        }

        public async Task DeletePrivilegeAsync(Privilege privilege)
        {
            _appDbContext.Privileges.Remove(privilege);

        }
    }
}
