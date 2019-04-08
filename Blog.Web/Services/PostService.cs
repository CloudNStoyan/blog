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
        /// <param name="id">The id of the post</param>
        /// <returns>PostModel filled with the data of the post from the database</returns>
        public PostModel GetPostById(int id)
        {
            var postPoco = this.Database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@postId;", new NpgsqlParameter("postId", id));
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
        /// Retrieves the latest N posts
        /// </summary>
        /// <param name="count">How many posts to retrieve</param>
        /// <returns>PostModels filled with the data of the posts from the database</returns>
        public PostModel[] GetLatestPosts(int count)
        {
            var postPocos = this.Database.Query<PostPoco>("SELECT * FROM posts ORDER BY post_id LIMIT @count;", new NpgsqlParameter("count", count));
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
        /// <param name="id">The post id</param>
        /// <returns>TagPocos from the database for this particular post</returns>
        private TagPoco[] GetPostTags(int id)
        {
            var tags = this.Database.Query<TagPoco>(
                "SELECT t.tag_id, t.tag_name FROM (SELECT * FROM posts_tags INNER JOIN posts ON posts_tags.post_id = @userId)" +
                " AS q INNER JOIN tags AS t ON q.tag_id = t.tag_id;",
                new NpgsqlParameter("userId", id));

            return tags.ToArray();
        }
    }
}
