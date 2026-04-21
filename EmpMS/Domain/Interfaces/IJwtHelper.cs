using System.Security.Claims;

namespace Domain.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username, string email, int? employeeId, List<string> permissions);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
