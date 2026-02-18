using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistAsync(string username);
        Task CreateUserAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
    }
}
