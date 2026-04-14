using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;

        public AuthRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> UserExistAsync(string username)
        {
            return await _appDbContext.Users.AnyAsync(u => u.Username == username);
        }
        public Task<bool> EmailExistsAsync(string email)
        {
            return _appDbContext.Users.AnyAsync(u => u.Email == email);
        }
        public Task<bool> EmployeeHasUserAsync(int employeeId)
        {
            return _appDbContext.Users.AnyAsync(u => u.EmployeeId == employeeId);
        }

        public async Task CreateUserAsync(User user)
        {
            await _appDbContext.Users.AddAsync(user);
        }
        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _appDbContext.UserRoles.AddAsync(userRole);

        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _appDbContext.Users
                .AsNoTracking()
                .Where(u => u.Id ==  id).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _appDbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _appDbContext.Users
                .AsNoTracking()
                .Where(u => u.Email == email).FirstOrDefaultAsync();
        }
        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            return await _appDbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePrivileges)
                .Select(rp => rp.Privilege.PrivilegeName)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _appDbContext.Users.Update(user);

        }


    }
}
