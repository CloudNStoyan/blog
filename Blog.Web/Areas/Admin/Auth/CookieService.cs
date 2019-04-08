using Microsoft.AspNetCore.Http;

namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// Handles cookie usings
    /// </summary>
    public class CookieService
    {
        private IHttpContextAccessor ContextAccessor { get; }

        /// <summary>
        /// Returns the HttpContext Response
        /// </summary>
        private HttpResponse Response => this.ContextAccessor.HttpContext.Response;

        /// <summary>
        /// Returns the HttpContext Request
        /// </summary>
        private HttpRequest Request => this.ContextAccessor.HttpContext.Request;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            this.ContextAccessor = contextAccessor;
        }

        /// <summary>
        /// Adds cookie
        /// </summary>
        /// <param name="key">Cookie name</param>
        /// <param name="value">Cookie value</param>
        public void SetCookie(string key, string value)
        {
            this.Response.Cookies.Append(key, value);
        }

        /// <summary>
        /// Retrives cookie
        /// </summary>
        /// <param name="key">Cookie name</param>
        /// <returns>Cookie value</returns>
        public string GetCookie(string key)
        {
            return this.Request.Cookies[key];
        }

        /// <summary>
        /// Removes cookie
        /// </summary>
        /// <param name="key">Cookie name</param>
        public void RemoveCookie(string key)
        {
            this.Response.Cookies.Delete(key);
        }
    }

    /// <summary>
    /// Handles session calls with the cookie
    /// </summary>
    public class SessionCookieService
    {
        private CookieService CookieService { get; }

        private const string CookieKey = "__session__";
        
        public SessionCookieService(CookieService cookieService)
        {
            this.CookieService = cookieService;
        }

        /// <summary>
        /// Adds the session key as a cookie
        /// </summary>
        /// <param name="sessionKey"></param>
        public void SetSessionKey(string sessionKey)
        {
            this.CookieService.SetCookie(CookieKey, sessionKey);
        }

        /// <summary>
        /// Gets the session key from the cookie
        /// </summary>
        /// <returns>The session key</returns>
        public string GetSessionKey()
        {
            return this.CookieService.GetCookie(CookieKey);
        }

        /// <summary>
        /// Removes session key from the cookie
        /// </summary>
        public void RemoveSessionKey()
        {
            this.CookieService.RemoveCookie(CookieKey);
        }
    }
}
