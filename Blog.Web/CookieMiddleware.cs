using System.Threading.Tasks;
using Autofac;
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
            string username = cookieService.ReadCookie("username");
            string password = cookieService.ReadCookie("password");

            if (username != null && password != null)
            {
                var loginAccountModel = new LoginAccountModel()
                {
                    Username = username,
                    Password = password
                };

                var container = MainContainer.Configure();

                using (var scope = container.BeginLifetimeScope())
                {
                    var authService = scope.Resolve<AuthenticationService>();


                    var confirm = authService.ConfirmAccount(loginAccountModel);
                    if (confirm != null)
                    {
                        var accountModel = new AccountModel()
                        {
                            Username = confirm.Name,
                            Password = confirm.Password,
                            Avatar = confirm.AvatarUrl
                        };

                        context.Items.Add("isLogged", true);
                        context.Items.Add("account", accountModel);
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
