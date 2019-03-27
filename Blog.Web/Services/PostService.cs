using System.Collections.Generic;
using System.Linq;
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

        public PostModel GetPostById(int id)
        {
            var postPoco = this.Database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i;", new NpgsqlParameter("i", id));
            var post = new PostModel
            {
                Content = postPoco.Content,
                Id = postPoco.PostId,
                Title = postPoco.Title
            };

            var tempCommentList = new List<Comment>();
            var commentsPoco = this.GetPostComments(postPoco.PostId);
            foreach (var commentPoco in commentsPoco)
            {
                var tempComment = new Comment
                {
                    Content = commentPoco.Content,
                    DateCreated = commentPoco.CreatedOn
                };

                var tempPocoUser = this.GetUser(commentPoco.UserId);
                var tempUser = new User
                {
                    AvatarUrl = tempPocoUser.AvatarUrl,
                    Name = tempPocoUser.Name
                };

                tempComment.User = tempUser;

                tempCommentList.Add(tempComment);
            }

            var tagsPoco = this.GetPostTags(postPoco.PostId);

            var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

            post.Tags = tempTags;

            return post;
        }

        public PostModel[] GetLatestPosts(int count)
        {
            var postPocos = this.Database.Query<PostPoco>("SELECT * FROM posts ORDER BY post_id LIMIT @c;", new NpgsqlParameter("c", count));
            var posts = new List<PostModel>();

            foreach (var postPoco in postPocos)
            {
                var post = new PostModel
                {
                    Content = postPoco.Content,
                    Id = postPoco.PostId,
                    Title = postPoco.Title
                };

                var tempCommentList = new List<Comment>();
                var commentsPoco = this.GetPostComments(postPoco.PostId);
                foreach (var commentPoco in commentsPoco)
                {
                    var tempComment = new Comment
                    {
                        Content = commentPoco.Content,
                        DateCreated = commentPoco.CreatedOn
                    };

                    var tempPocoUser = this.GetUser(commentPoco.UserId);
                    var tempUser = new User
                    {
                        AvatarUrl = tempPocoUser.AvatarUrl,
                        Name = tempPocoUser.Name
                    };

                    tempComment.User = tempUser;

                    tempCommentList.Add(tempComment);
                }

                var tagsPoco = this.GetPostTags(postPoco.PostId);

                var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

                post.Tags = tempTags;

                posts.Add(post);
            }

            return posts.ToArray();
        }


        private UserPoco GetUser(int id)
        {
            var user = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i;", new NpgsqlParameter("i", id));
            return user;
        }

        private CommentPoco[] GetPostComments(int id)
        {
            var comments = this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i;", new NpgsqlParameter("i", id));
            return comments.ToArray();
        }

        private TagPoco[] GetPostTags(int id)
        {
            var tags = this.Database.Query<TagPoco>(
                "SELECT t.tag_id, t.name FROM (SELECT * FROM posts_tags INNER JOIN posts ON posts_tags.post_id = @i)" +
                " AS q INNER JOIN tags AS t ON q.tag_id = t.tag_id;",
                new NpgsqlParameter("i", id));

            return tags.ToArray();
        }
    }
}
