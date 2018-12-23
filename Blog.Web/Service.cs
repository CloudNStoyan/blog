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
            var poststags = this.Database.Query<PostsTagsPoco>("SELECT * FROM posts_tags WHERE post_id=@i;", new NpgsqlParameter("i", id));
            var tags = new List<TagPoco>();

            this.Database.Query<>()

            return tags.ToArray();
        }

        public CommentPoco[] GetComments(int id)
        {
            var comments = this.Database.Query<CommentPoco>("SELECT * FROM ")
        }
    }
}
