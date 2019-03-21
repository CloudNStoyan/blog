using Blog.Web.Services;
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

        public void SetCookie(string key, string value)
        {
            this.HttpContext.SetCookie(key, value);
        }

        public string ReadCookie(string key)
        {
            return this.HttpContext.GetCookie(key);
        }

        public void DeleteCookie(string key)
        {
            this.HttpContext.DeleteCookie(key);
        }
    }
}
