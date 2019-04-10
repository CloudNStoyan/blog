using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// Authenticates user by checking if the cookies contains session and if does check if its valid and,
    /// if everything goes right will give session to the context.
    /// </summary>
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
                IsLoggedIn = true,
                SessionId = session.LoginSessionId,
                UserAccount = new AccountModel
                {
                    Avatar = pocoUser.AvatarUrl,
                    UserId = pocoUser.UserId,
                    Username = pocoUser.Name
                }
            });

            var identity = new ClaimsIdentity("Custom");
            identity.AddClaim(new Claim(ClaimTypes.Name, pocoUser.Name));
            context.User = new ClaimsPrincipal(identity);

            await this.next.Invoke(context);
        }
    }

    /// <summary>
    /// This contains the session that the current user is connected with.
    /// </summary>
    public class RequestSession
    {
        public bool IsLoggedIn { get; set; }

        public AccountModel UserAccount { get; set; }

        public int SessionId { get; set; }
    }

    public class AccountModel
    {
        public string Username { get; set; }

        public int UserId { get; set; }

        public string Avatar { get; set; }
    }

    /// <summary>
    /// Connects the AuthMiddleware to the pipe.
    /// </summary>
    public static class MyMiddleWareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
