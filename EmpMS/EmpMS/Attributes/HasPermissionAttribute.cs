using Microsoft.AspNetCore.Authorization;

namespace EmpMS.Attributes
{
    // A simple attribute that marks "this endpoint requires permission X"
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
        : base(policy: permission) // uses ASP.NET policy system
        {
        }
    }
}
