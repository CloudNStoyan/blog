using Microsoft.AspNetCore.Http;

namespace Blog.Web.Areas.Admin.Auth
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CookieService
    {
        private IHttpContextAccessor ContextAccessor { get; }

        private HttpResponse Response => this.ContextAccessor.HttpContext.Response;

        private HttpRequest Request => this.ContextAccessor.HttpContext.Request;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            this.ContextAccessor = contextAccessor;
        }

        public void SetCookie(string key, string value)
        {
            this.Response.Cookies.Append(key, value);
        }

        public string GetCookie(string key)
        {
            return this.Request.Cookies[key];
        }

        public void RemoveCookie(string key)
        {
            this.Response.Cookies.Delete(key);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class SessionCookieService
    {
        private CookieService CookieService { get; }

        private const string CookieKey = "__session__";
        
        public SessionCookieService(CookieService cookieService)
        {
            this.CookieService = cookieService;
        }

        public void SetSessionKey(string sessionKey)
        {
            this.CookieService.SetCookie(CookieKey, sessionKey);
        }

        public string GetSessionKey()
        {
            return this.CookieService.GetCookie(CookieKey);
        }

        public void RemoveSessionKey()
        {
            this.CookieService.RemoveCookie(CookieKey);
        }
    }
}
