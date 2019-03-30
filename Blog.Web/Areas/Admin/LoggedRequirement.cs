using Microsoft.AspNetCore.Authorization;

namespace Blog.Web.Areas.Admin
{
    public class LoggedRequirement : IAuthorizationRequirement
    {
        public bool Logged { get; }

        public LoggedRequirement(bool logged)
        {
            this.Logged = logged;
        }
    }
}
