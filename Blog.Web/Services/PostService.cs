using System.Collections.Generic;
using System.Linq;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Services
{
    public class PostService
    {
        private Database Database { get; set; }

        public PostService(Database database)
        {
            this.Database = database;
        }

        /// <summary>
        /// Get post from the database with id
        /// </summary>
        public PostModel GetPostById(int id)
        {
            var postPoco = this.Database.QueryOne<PostPoco>("SELECT * FROM posts AS post WHERE post.post_id=@postId;", new NpgsqlParameter("postId", id));
            var post = new PostModel
            {
                Content = postPoco.Content,
                Id = postPoco.PostId,
                Title = postPoco.Title
            };

            var tagsPoco = this.GetPostTags(postPoco.PostId);

            var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

            post.Tags = tempTags;

            return post;
        }

        /// <summary>
        /// Retrieves the latest posts
        /// </summary>
        public PostModel[] GetLatestPosts(int count)
        {
            var postPocos = this.Database.Query<PostPoco>("SELECT * FROM posts AS post ORDER BY post.post_id LIMIT @count;", new NpgsqlParameter("count", count));
            var posts = new List<PostModel>();

            foreach (var postPoco in postPocos)
            {
                var post = new PostModel
                {
                    Content = postPoco.Content,
                    Id = postPoco.PostId,
                    Title = postPoco.Title
                };

                var tagsPoco = this.GetPostTags(postPoco.PostId);

                var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

                post.Tags = tempTags;

                posts.Add(post);
            }

            return posts.ToArray();
        }

        /// <summary>
        /// Retrieves tags from post id
        /// </summary>
        private TagPoco[] GetPostTags(int id)
        {
            var tags = this.Database.Query<TagPoco>(
                "SELECT tag.tag_id, tag.tag_name FROM (SELECT * FROM posts_tags INNER JOIN posts ON posts_tags.post_id = @userId)" +
                " AS query INNER JOIN tags AS tag ON query.tag_id = tag.tag_id;",
                new NpgsqlParameter("userId", id));

            return tags.ToArray();
        }
    }
}
