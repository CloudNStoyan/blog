using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web
{
    public class Service
    {
        public const string ConnectionString = @"Server=vm13.lan;Port=4401;Database=blog;Uid=blog;Pwd=test123;";
        private Database Database { get; }

        public Service(Database database)
        {
            this.Database = database;
        }

        public PostPoco GetPost(int id)
        {
            var post = this.Database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i;", new NpgsqlParameter("i", id));

            return post;
        }

        public TagPoco[] GetTags(int id)
        {
            var tags = this.Database.Query<TagPoco>(
                "SELECT t.tag_id, t.name FROM (SELECT * FROM posts_tags INNER JOIN posts ON posts_tags.post_id = @i) AS q INNER JOIN tags AS t ON q.tag_id = t.tag_id;",
                new NpgsqlParameter("i", id));

            return tags.ToArray();
        }

        public CommentPoco[] GetComments(int id)
        {
            var comments = this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i;", new NpgsqlParameter("i", id));
            return comments.ToArray();
        }

        public UserPoco GetUser(int id)
        {
            var user = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i;", new NpgsqlParameter("i", id));
            return user;
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

                var tags = this.GetTags(postPoco.PostId);

                lightPostModel.Tags = tags.Select(tagPoco => tagPoco.Name).ToArray();

                finishedModels.Add(lightPostModel);
            }

            return finishedModels;
        }

        public UserPoco ConfirmAccount(LoginAccountModel loginModel)
        {
            var parametars = new[]
            {
                new NpgsqlParameter("u", loginModel.Username), new NpgsqlParameter("p", loginModel.Password)

            };

            var account = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE username=@u AND password=@p", parametars);

            return account;

        }

    }
}
