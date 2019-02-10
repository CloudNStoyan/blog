using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.DAL;
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
    }
}
