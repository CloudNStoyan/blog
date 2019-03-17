using Microsoft.AspNetCore.Http;

namespace Blog.Web
{
    public class CookieService
    {
        private HttpContext HttpContext { get; set; }

        public CookieService(HttpContext httpContext)
        {
            this.HttpContext = httpContext;
        }

        public void SetCookie(string key, string value, CookieOptions option)
        {
            this.HttpContext.Response.Cookies.Append(key, value, option);
        }

        public string ReadCookie(string key)
        {
            return this.HttpContext.Request.Cookies[key];
        }

        public void DeleteCookie(string key)
        {
            this.HttpContext.Response.Cookies.Delete(key);
        }
    }
}
