using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Blog.Web.DAL;
using Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Blog.Web.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Post(int id)
        {
            var container = MainContainer.Configure();
            var service = container.Resolve<PostService>();

            var post = service.GetPostById(id);

            return View(post);
        }
    }
}