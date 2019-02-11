using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web
{
    public class PostService
    {
        private Database Database { get; set; }

        public PostService(Database database)
        {
            this.Database = database;
        }

        public PostPoco GetPostById(int id)
        {
            var post = this.Database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i;", new NpgsqlParameter("i", id));

            return post;
        }

        

        public List<LightPostModel> GetLatest(int count)
        {
            var posts = this.Database.Query<PostPoco>("SELECT * FROM posts ORDER BY post_id LIMIT @l;", new NpgsqlParameter("l", count));
            var finishedModels = new List<LightPostModel>();
            foreach (var postPoco in posts)
            {
                var lightPostModel = new LightPostModel
                {
                    Content = string.Join(" ", postPoco.Content.Split(' ').Take(8)),
                    PostId = postPoco.PostId,
                    Title = postPoco.Title
                };

                //var tags = this.GetTags(postPoco.PostId);

                //lightPostModel.Tags = tags.Select(tagPoco => tagPoco.Name).ToArray();

                finishedModels.Add(lightPostModel);
            }

            return finishedModels;
        }
    }
}
