﻿using Blog.Web.Areas.Admin;
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

        public RequestSession Session => this.ContextAccessor.HttpContext.GetSession();
    }

    public static class ExtensionSessionService
    {
        private const string SessionKey = "__session__";

        public static RequestSession GetSession(this HttpContext context)
        {
            return (RequestSession) context.Items[SessionKey];
        }

        public static void SetSession(this HttpContext context, RequestSession session)
        {
            context.Items[SessionKey] = session;
        }
    }
}