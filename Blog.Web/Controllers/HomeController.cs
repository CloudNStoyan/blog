using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Blog.Web.DAL;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var posts = new List<LightPostModel>();

            //using (var conn = new NpgsqlConnection(Service.ConnectionString))
            //{
            //    var database = new Database(conn);
            //    var service = new Service(database);
            //    posts = service.GetLatest(10);
            //}

            var container = MainContainer.Configure();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IService>();
                posts = service.GetLatest(10);
            }

            return View(posts.ToArray());
        }

        public IActionResult LoginPage()
        {
            this.ViewData.Add("isLogged", this.HttpContext.Items["isLogged"]);
            if (this.HttpContext.Items["account"] != null)
            {
                return this.View((LoginAccountModel) this.HttpContext.Items["account"]);
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
