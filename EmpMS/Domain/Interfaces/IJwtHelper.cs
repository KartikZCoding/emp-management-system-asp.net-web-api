using System.Security.Claims;

namespace Domain.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username, string email, List<string> permissions);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
