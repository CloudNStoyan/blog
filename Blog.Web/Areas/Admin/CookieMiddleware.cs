using System;
using System.Threading.Tasks;
using Autofac;
using Blog.Web.Areas.Admin.Models;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Blog.Web
{
    public class CookieMiddleware
    {
        private RequestDelegate next;

        public CookieMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var cookieService = new CookieService(context);
            string sessionKey = cookieService.ReadCookie("session_key");

            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                var container = ContainerFactory.Create();

                using (var scope = container.BeginLifetimeScope())
                {
                    var service = scope.Resolve<AuthenticationService>();
                    var session = service.RequestSession(sessionKey);

                    var loginTime = session.LoginTime;
                    loginTime = loginTime.AddMinutes(30);

                    if (loginTime > DateTime.Now)
                    {
                        var userPoco = service.GetAccountPoco(session.UserId);

                        context.Items.Add("isLogged", true);

                        var acc = new AccountModel()
                        {
                            Username = userPoco.Name,
                            Avatar = userPoco.AvatarUrl,
                        };

                        context.Items.Add("account", acc);
                    }
                    else
                    {
                        context.Items.Add("isLogged", false);
                    }
                }
            }
            else
            {
                context.Items.Add("isLogged", false);
            }

            await this.next.Invoke(context);
        }
    }

    public static class MyMiddleWareExtensions
    {
        public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieMiddleware>();
        }
    }
}
