using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
using Npgsql;

namespace Blog.Web
{
    public class Service
    {
        public const string ConnectionString = @"";
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
            var comments = this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i", new NpgsqlParameter("i", id));
            return comments.ToArray();
        }

        public UserPoco GetUser(int id)
        {
            var user = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i", new NpgsqlParameter("i", id));
            return user;
        }
    }
}
