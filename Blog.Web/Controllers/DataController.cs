﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            PostPoco rawPost;
            TagPoco[] rawTags;
            CommentPoco[] rawComments;

            using (var conn = new NpgsqlConnection(Service.ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                rawPost = service.GetPost(id);
                rawTags = service.GetTags(id);
                rawComments = service.GetComments(id);
            }

            var tags = new List<string>();

            var comments = new List<Comment>();

            foreach (var rawComment in rawComments)
            {
                var comment = new Comment
                {
                    Content = rawComment.Content,
                    DateCreated = rawComment.CreatedOn
                };
                var user = new User();
                UserPoco rawUser;

                using (var conn = new NpgsqlConnection(Service.ConnectionString))
                {
                    var database = new Database(conn);
                    var service = new Service(database);
                    rawUser = service.GetUser(rawComment.UserId);
                    Console.WriteLine($"\n {rawUser.Name} {rawUser.AvatarUrl} \n");
                }

                user.Name = rawUser.Name;
                user.AvatarUrl = rawUser.AvatarUrl;
                comment.User = user;

                comments.Add(comment);
            }
            

            foreach (var tagPoco in rawTags)
            {
                tags.Add(tagPoco.Name);
            }

            var post = new PostModel()
            {
                Title = rawPost.Title,
                Content = rawPost.Content,
                Tags = tags.ToArray(),
                Comments = comments.ToArray()
            };

            return View(post);
        }
    }
}