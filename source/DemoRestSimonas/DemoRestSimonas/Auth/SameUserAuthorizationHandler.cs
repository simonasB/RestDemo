using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoRestSimonas.Auth.Model;
using Microsoft.AspNetCore.Authorization;

namespace DemoRestSimonas.Auth
{
    public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement, IUserOwnedResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement,
            IUserOwnedResource resource)
        {
            if (context.User.IsInRole(DemoRestUserRoles.Admin) || context.User.FindFirst(CustomClaims.UserId).Value == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public record SameUserRequirement : IAuthorizationRequirement;
}
