using System;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var container = MainContainer.Configure();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<PostService>();
                var posts = service.GetLatestPosts(10);

                return View(posts);
            }
        }

        public IActionResult LoginPage()
        {
            this.ViewData.Add("isLogged", this.HttpContext.Items["isLogged"]);
            if (this.HttpContext.Items["account"] != null)
            {
                return this.View((AccountModel) this.HttpContext.Items["account"]);
            }

            return this.View();
        }

        public IActionResult Login(LoginAccountModel account)
        {
            var cookieService = new CookieService(this.HttpContext);

            var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(10) };

            cookieService.SetCookie("Username", account.Username, option);
            cookieService.SetCookie("Password", account.Password, option);

            return this.Redirect("Index");
        }

        public IActionResult LogOut()
        {
            var cookieService = new CookieService(this.HttpContext);
            cookieService.DeleteCookie("Username");
            cookieService.DeleteCookie("Password");
            return this.Redirect("LoginPage");
        }

    }
}
