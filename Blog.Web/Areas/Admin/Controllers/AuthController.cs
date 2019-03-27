﻿using Blog.Web.Areas.Admin.Services;
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

        private SessionCookieService SessionCookieService { get; }

        public AuthController(AuthenticationService authService, SessionService sessionService, SessionCookieService sessionCookieService)
        {
            this.AuthService = authService;
            this.SessionService = sessionService;
            this.SessionCookieService = sessionCookieService;
        }

        public IActionResult Login(LoginAccountModel account)
        {
            if (string.IsNullOrWhiteSpace(account.Username) || string.IsNullOrWhiteSpace(account.Password))
            {
                return this.RedirectToAction("Index", "Home", new { area = "" });
            }

            var user = this.AuthService.Login(account);

            if (user == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            string sessionKey = this.AuthService.CreateSession(user.UserId, account.RememberMe);

            this.SessionCookieService.SetSessionKey(sessionKey);

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            var session = this.SessionService.Session;

            this.AuthService.Logout(session);

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