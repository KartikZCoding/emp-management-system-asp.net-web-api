using EmpMS.Models;

namespace EmpMS.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> UserExistAsync(string  username);
    }
}
