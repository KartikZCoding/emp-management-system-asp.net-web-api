using EmpMS.Data;
using EmpMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;

        public AuthRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task CreateUserAsync(User user)
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _appDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _appDbContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> UserExistAsync(string username)
        {
            return await _appDbContext.Users.AnyAsync(u => u.Username == username);
        }
    }
}
