using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EmpMS.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Dynamically create a policy for ANY permission name
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Required interface methods — delegate to default
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackProvider.GetFallbackPolicyAsync();
        }
    }
}
