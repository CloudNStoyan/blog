using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Models;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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
        public async Task Invoke(HttpContext context, AuthenticationService authService, SessionCookieService sessionCookieService) {
            context.SetSession(new RequestSession());

            string sessionKey = sessionCookieService.GetSessionKey();

            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                await this.next.Invoke(context);
                return;
            }

            var session = authService.GetSessionBySessionKey(sessionKey);

            var now = DateTime.Now;

            if (session == null || session.LoggedOut || session.ExpirationDate <= now)
            {
                await this.next.Invoke(context);
                return;
            }

            var pocoUser = authService.GetUserById(session.UserId);

            context.SetSession(new RequestSession
            {
                IsLogged = true,
                SessionId = session.LoginSessionId,
                UserAccount = new AccountModel
                {
                    Avatar = pocoUser.AvatarUrl,
                    UserId = pocoUser.UserId,
                    Username = pocoUser.Name
                }
            });

            var claims = new List<Claim> {new Claim(ClaimTypes.Name, pocoUser.Name, ClaimValueTypes.String)};
            var userIdentity = new ClaimsIdentity("Admins");
            userIdentity.AddClaims(claims);

            context.User.AddIdentity(userIdentity);

            context.User.FindAll("Logged");

            await this.next.Invoke(context);
        }
    }

    public class RequestSession
    {
        public bool IsLogged { get; set; }

        public AccountModel UserAccount { get; set; }

        public int SessionId { get; set; }
     
    }

    public static class MyMiddleWareExtensions
    {
        public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
