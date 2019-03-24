using System.Security.Cryptography;
using System.Text;
using Autofac;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.Models;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private AuthenticationService AuthService { get; }
        private SessionService SessionService { get; }

        public AuthController(AuthenticationService authService, SessionService sessionService)
        {
            this.AuthService = authService;
            this.SessionService = sessionService;
        }

        public IActionResult Login(LoginAccountModel account)
        {
            if (!string.IsNullOrWhiteSpace(account.Username) && !string.IsNullOrWhiteSpace(account.Password))
            {
                var cookieService = new CookieService(this.HttpContext);

                var confirmedAccount = this.AuthService.ConfirmAccount(account);

                if (confirmedAccount != null)
                {
                    string session = this.AuthService.MakeSession(confirmedAccount.UserId, !account.RememberMe);

                    cookieService.SetCookie("sessionKey", session);
                }

                return this.RedirectToAction("Index", "Home");
            }

            return this.RedirectToAction("Index", "Home", new { area = ""});
        }

        public IActionResult LogOut()
        {
            var session = this.SessionService.Session;
            this.AuthService.DeleteSession(session);

            return this.Redirect("LoginPage");
        }

        public IActionResult LoginPage()
        {
            var session = this.SessionService.Session;

            if (session.IsLogged)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }
    }
}