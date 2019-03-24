using Blog.Web.Areas.Admin;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.Services
{
    public class SessionService
    {
        private IHttpContextAccessor ContextAccessor { get; }

        public SessionService(IHttpContextAccessor contextAccessor)
        {
            this.ContextAccessor = contextAccessor;
        }

        public Session Session => this.ContextAccessor.HttpContext.GetSession();
    }

    public static class ExtensionSessionService
    {
        private const string SessionKey = "__session__";

        public static Session GetSession(this HttpContext context)
        {
            return (Session) context.Items[SessionKey];
        }

        public static void SetSession(this HttpContext context, Session session)
        {
            context.Items[SessionKey] = session;
        }
    }

    public static class ExtensionCookies
    {
        public static string GetCookie(this HttpContext context, string key)
        {
            return context.Request.Cookies[key];
        }

        public static void SetCookie(this HttpContext context, string key, string value)
        {
            context.Response.Cookies.Append(key, value);
        }

        public static void DeleteCookie(this HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }
    }
}
