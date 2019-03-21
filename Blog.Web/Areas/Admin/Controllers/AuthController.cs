using System;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using Blog.Web.Areas.Admin.Models;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.DAL;
using Blog.Web.Models;
using Blog.Web.Services;
using Microsoft.AspNetCore.Http;
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
            var cookieService = new CookieService(this.HttpContext);

            var confirmedAccount = this.AuthService.ConfirmAccount(account);

            if (confirmedAccount != null)
            {
                string session = this.AuthService.MakeSession(confirmedAccount.UserId, !account.RememberMe);

                cookieService.SetCookie("sessionKey", session);
            }

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            var session = this.SessionService.Session;
            this.AuthService.DeleteSession(session);

            return this.Redirect("LoginPage");
        }

        public IActionResult Register(LoginAccountModel account)
        {
            var container = ContainerFactory.Create();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<AuthenticationService>();

                byte[] passwordBytes = Encoding.ASCII.GetBytes(account.Password);
                byte[] result;

                using (var shaM = new SHA512Managed())
                {
                    result = shaM.ComputeHash(passwordBytes);
                }

                var registerModel = new RegisterModel()
                {
                    AvatarUrl = "none",
                    Password = result,
                    Username = account.Username
                };

                service.CreateAccount(registerModel);
            }

            return this.RedirectToAction("Index", "Home");
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