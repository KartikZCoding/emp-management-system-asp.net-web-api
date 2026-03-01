

namespace Application.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username, string email, string rolename);
    }
}
