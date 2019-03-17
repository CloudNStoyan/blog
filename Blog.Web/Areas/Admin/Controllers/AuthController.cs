using System;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using Blog.Web.Areas.Admin.Models;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        public IActionResult Login(LoginAccountModel account)
        {
            var cookieService = new CookieService(this.HttpContext);

            var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(30) };

            cookieService.SetCookie("Username", account.Username, option);
            cookieService.SetCookie("Password", account.Password, option);

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            var cookieService = new CookieService(this.HttpContext);
            cookieService.DeleteCookie("Username");
            cookieService.DeleteCookie("Password");
            return this.Redirect("LoginPage");
        }

        public IActionResult Register(LoginAccountModel account)
        {
            var container = MainContainer.Configure();

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
            this.ViewData.Add("isLogged", this.HttpContext.Items["isLogged"]);
            if (this.HttpContext.Items["account"] != null)
            {
                return this.View((AccountModel)this.HttpContext.Items["account"]);
            }

            return this.View();
        }
    }
}