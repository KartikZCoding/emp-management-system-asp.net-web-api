using EmpMS.Data;
using EmpMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMS.Repositories
{
    public class RolePrivilegeRepository : IRolePrivilegeRepository
    {
        private readonly AppDbContext _appDbContext;

        public RolePrivilegeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AssignPrivilegeToRoleAsync(RolePrivilege rolePrivilege)
        {
            await _appDbContext.RolePrivileges.AddAsync(rolePrivilege);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<RolePrivilege>> GetPrivilegesByRoleIdAsync(int roleId)
        {
            return await _appDbContext.RolePrivileges.Include(rp => rp.Privilege).Where(rp => rp.RoleId == roleId).ToListAsync();
        }

        public async Task RemoveRolePrivilegeAsync(int id)
        {
            var rolePrivilege = await _appDbContext.RolePrivileges.Where(rp => rp.Id == id).FirstOrDefaultAsync();
            _appDbContext.RolePrivileges.Remove(rolePrivilege);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
