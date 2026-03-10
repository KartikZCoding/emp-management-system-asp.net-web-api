using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExistAsync(string username);
        Task CreateUserAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<string>> GetUserPermissionsAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
