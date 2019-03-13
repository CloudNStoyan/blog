using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login(LoginAccountModel account)
        {
            var cookieService = new CookieService(this.HttpContext);

            var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(30) };

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

        public IActionResult Register(LoginAccountModel account)
        {


            return this.Redirect("LoginPage");
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