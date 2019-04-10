using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// Its created for the authorize attribute. If session is provided will fill the Authentication which authroize attribute is for.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private SessionService SessionService { get; }

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder,
            ISystemClock clock,
            SessionService sessionService) : base(options, logger, encoder, clock)
        {
            this.SessionService = sessionService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var session = this.SessionService.Session;

            if (!session.IsLoggedIn)
            {
                return Task.FromResult(AuthenticateResult.Fail("Not logged!"));
            }

            var principal = new ClaimsPrincipal(this.Context.User.Identity);
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        /// <summary>
        /// If something went wrong redirects to this method.
        /// </summary>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            this.Context.Response.Redirect("/Admin/Auth/LoginPage");

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Adds the authentication handler to the main builder
    /// </summary>
    public static class CustomAuthenticationExtension
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(authenticationScheme, null);
        }
    }
}
