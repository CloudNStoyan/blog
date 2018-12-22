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
                    Title = "My second Post",
                    Content = "This is the contnet \n\n CoNtEnT",
                    Tags = new [] {"Tags", "And", "Another", "One"},
                    Comments = new []{ new Comment() {Content = "This comment comes from the DB" , DateCreated = "2018, September 1, 23:00", User = new User()
                    {
                        AvatarUrl = "https://static.u.gg/lol/riot_static/8.24.1/img/spell/SummonerFlash.png",
                        Name = "Stoyan"
                    }}}
                }

            };


            return View(posts[id]);
        }
    }
}