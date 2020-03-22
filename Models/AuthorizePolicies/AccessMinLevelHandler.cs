using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.AuthorizePolicies
{
    public class AccessMinLevelHandler : AuthorizationHandler<AccessMinLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessMinLevelRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                AccessLevelUserRolesEnum userRole = (AccessLevelUserRolesEnum)Enum.Parse(typeof(AccessLevelUserRolesEnum), context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value);
                if (userRole >= requirement.Role)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
