using System;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        public PostService PostService { get; set; }

        public HomeController(PostService postService)
        {
            this.PostService = postService;
        }

        public IActionResult Index()
        {
            //var container = ContainerFactory.Create();
            //
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var service = scope.Resolve<PostService>();
            //    var posts = service.GetLatestPosts(10);
            //
            //    return View(posts);
            //}

            var posts = this.PostService.GetLatestPosts(10);

            return this.View(posts);
        }
    }
}
