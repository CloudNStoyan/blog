using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Post(int id)
        {
            var posts = new[] {
                new PostModel
            {
                Title = "My first Post",
                Content = "This is the contnet \n\n CoNtEnT",
                Tags = new [] {"Tags", "And", "Another", "One"}
            },
                new PostModel
            {
                Title = "My first Post",
                Content = "This is the contnet \n\n CoNtEnT",
                Tags = new [] {"Tags", "And", "Another", "One"}
            }

            };


            return View(posts[0]);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
