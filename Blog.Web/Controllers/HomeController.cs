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
            var container = ContainerFactory.Create();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<PostService>();
                var posts = service.GetLatestPosts(10);

                return View(posts);
            }
        }
    }
}
