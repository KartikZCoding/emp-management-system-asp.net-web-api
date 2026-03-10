using Microsoft.AspNetCore.Authorization;

namespace EmpMS.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
        {
            // Get all "Permission" claims from the JWT
            var userPermissions = context.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value);
            // Check if the required permission exists
            if (userPermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);  // ✅ authorized
            }
            // If not found, do nothing → ASP.NET returns 403 Forbidden
            return Task.CompletedTask;
        }
    }
}
