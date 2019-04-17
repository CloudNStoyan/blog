using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.ErrorHandling
{
    public class NotFoundMiddleware
    {
        private RequestDelegate next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await this.next.Invoke(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.Redirect("/error/pagenotfound");
            }
        }
    }

    public static class NotFoundMiddlewareExtensions
    {
        /// <summary>
        /// Returns custom 404 not found page if 404 error occurs.
        /// </summary>
        public static IApplicationBuilder UseNotFoundMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NotFoundMiddleware>();
        }
    }
}
