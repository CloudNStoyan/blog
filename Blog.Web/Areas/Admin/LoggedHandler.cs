using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Web.Areas.Admin
{
    public class LoggedHandler : AuthorizationHandler<LoggedRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            LoggedRequirement requirement)
        {
            var user = context.User.FindFirst(c => c.Type == ClaimTypes.Name);

            if (user != null)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
