using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace Blog.Web.ErrorHandling
{
    public class InternalErrorMiddleware
    {
        private RequestDelegate next;

        public InternalErrorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Exception error = null;

            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            {
                context.Response.Redirect("/error/somethingwentwrong?error=" + error?.Message);
            }
        }
    }

    public static class InternalErrorMiddlewareExtensions
    {
        /// <summary>
        /// Returns custom 5XX internal error page if 5XX error occurs.
        /// </summary>
        public static IApplicationBuilder UseInternalErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InternalErrorMiddleware>();
        }
    }
}