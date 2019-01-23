using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
using Blog.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;

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

                using (var conn = new NpgsqlConnection(Service.ConnectionString))
                {
                    var database = new Database(conn);
                    var service = new Service(database);
                    var confirm = service.ConfirmAccount(loginAccountModel);

		    loginAccountModel.Avatar = confirm.AvatarUrl;

                    context.Items.Add("isLogged", confirm != null);
		    context.Items.Add("account", loginAccountModel);
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
