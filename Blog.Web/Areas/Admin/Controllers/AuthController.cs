using Blog.Web.Areas.Admin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        public IActionResult Login(LoginDataModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.Password))
            {
                return this.RedirectToAction("Index", "Home", new { area = "" });
            }

            var user = this.AuthService.Login(data);

            if (user == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            string sessionKey = this.AuthService.CreateNewSession(user.UserId, data.RememberMe);

            this.SessionCookieService.SetSessionKey(sessionKey);

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            var session = this.SessionService.Session;

            this.AuthService.Logout(session);

            this.SessionCookieService.RemoveSessionKey();

            return this.Redirect("LoginPage");
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