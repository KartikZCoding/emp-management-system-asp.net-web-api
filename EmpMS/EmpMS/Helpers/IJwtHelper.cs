using EmpMS.Models;

namespace EmpMS.Helpers
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username, string rolename);
    }
}
