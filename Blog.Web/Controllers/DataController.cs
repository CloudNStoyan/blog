using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class DataController : Controller
    {
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


            return View(posts[id]);
        }
    }
}