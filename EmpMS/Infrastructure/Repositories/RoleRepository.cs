using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _appDbContext;

        public RoleRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _appDbContext.Roles.AnyAsync(r => r.RoleName == roleName);
        }

        public async Task CreateRoleAsync(Role role)
        {
            await _appDbContext.Roles.AddAsync(role);
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
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _appDbContext.Roles.Where(r => r.RoleName == roleName).FirstOrDefaultAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _appDbContext.Roles.Update(role);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Role role)
        {
            _appDbContext.Roles.Remove(role);
            await _appDbContext.SaveChangesAsync();
        }

    }
}
