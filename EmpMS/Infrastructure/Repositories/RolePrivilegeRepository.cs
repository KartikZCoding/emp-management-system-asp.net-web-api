using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RolePrivilegeRepository : IRolePrivilegeRepository
    {
        private readonly AppDbContext _appDbContext;

        public RolePrivilegeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddRolePrivilegeAsync(RolePrivilege rolePrivilege)
        {
            await _appDbContext.RolePrivileges.AddAsync(rolePrivilege);

        }

        public async Task<List<Privilege>> GetPrivilegesByRoleIdAsync(int roleId)
        {
            return await _appDbContext.RolePrivileges
                .Include(rp => rp.Privilege)
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Privilege)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RolePrivilege> GetRolePrivilegeAsync(int roleId, int privilegeId)
        {
            return await _appDbContext.RolePrivileges
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PrivilegeId == privilegeId);
        }

        public async Task<bool> RolePrivilegeExistsAsync(int roleId, int privilegeId)
        {
            return await _appDbContext.RolePrivileges
                .AnyAsync(rp => rp.RoleId == roleId && rp.PrivilegeId == privilegeId);
        }

        public async Task DeleteRolePrivilegeAsync(RolePrivilege rolePrivilege)
        {
            _appDbContext.RolePrivileges.Remove(rolePrivilege);

        }
    }
}
