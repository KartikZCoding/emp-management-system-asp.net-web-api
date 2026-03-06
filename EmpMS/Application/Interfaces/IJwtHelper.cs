

using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username, string email, string rolename);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
