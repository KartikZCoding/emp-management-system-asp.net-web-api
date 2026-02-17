using EmpMS.Data;
using EmpMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMS.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _appDbContext;

        public RoleRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task CreateRoleAsync(Role role)
        {
            await _appDbContext.Roles.AddAsync(role);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _appDbContext.Roles.Where(u => u.Id == id).FirstOrDefaultAsync();
            _appDbContext.Roles.Remove(role);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _appDbContext.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _appDbContext.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _appDbContext.Roles.AnyAsync(r=> r.RoleName == roleName);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _appDbContext.Roles.Update(role);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
