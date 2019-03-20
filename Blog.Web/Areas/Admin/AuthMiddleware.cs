using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Models;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.Areas.Admin
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AuthMiddleware
    {
        private readonly RequestDelegate next;

        public AuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        
        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context, AuthenticationService authService)
        {
            var cookieService = new CookieService(context);

            string sessionKey = cookieService.ReadCookie("sessionKey");

            var session = new Session();

            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                var pocoSession = authService.RequestSession(sessionKey);

                var pocoUser = authService.GetAccountPoco(pocoSession.UserId);

                if (pocoUser != null)
                {
                    session.UserAccount = new AccountModel
                    {
                        Avatar = pocoUser.AvatarUrl,
                        Id = pocoUser.UserId,
                        Username = pocoUser.Name
                    };
                    session.IsLogged = true;
                }
            }

            context.SetSession(session);

            await this.next.Invoke(context);
        }
    }

    public class Session
    {
        public bool IsLogged { get; set; }

        public AccountModel UserAccount { get; set; }
     
    }

    public static class MyMiddleWareExtensions
    {
        public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
