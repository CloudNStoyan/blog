using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var posts = new List<LightPostModel>();

            using (var conn = new NpgsqlConnection(Service.ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                posts = service.GetLatest(10);
            }

            return View(posts.ToArray());
        }
    }
}
