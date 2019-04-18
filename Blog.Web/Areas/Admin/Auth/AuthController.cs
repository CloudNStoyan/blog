using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Auth
{
    [Area(Roles.Admin)]
    public class AuthController : Controller
    {
        private AuthenticationService AuthService { get; }

        private SessionService SessionService { get; }

        private SessionCookieService SessionCookieService { get; }

        public AuthController(AuthenticationService authService, SessionService sessionService, SessionCookieService sessionCookieService)
        {
            this.AuthService = authService;
            this.SessionService = sessionService;
            this.SessionCookieService = sessionCookieService;
        }

        public async Task<IActionResult> Login(LoginDataModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.Password))
            {
                return this.RedirectToAction("Index", "Home", new { area = "" });
            }

            var user = await this.AuthService.Login(data);

            if (user == null)
            {
                return this.RedirectToAction("Index", "Home", new { area = "" });
            }

            string sessionKey = await this.AuthService.CreateNewSession(user.UserId, data.RememberMe);

            this.SessionCookieService.SetSessionKey(sessionKey);

            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            var session = this.SessionService.Session;

            await this.AuthService.Logout(session);

            this.SessionCookieService.RemoveSessionKey();

            return this.RedirectToAction("LoginPage", "Auth");
        }

        public IActionResult LoginPage()
        {
            var session = this.SessionService.Session;

            if (session.IsLoggedIn)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }
    }
}