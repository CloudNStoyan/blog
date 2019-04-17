using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PostService
    {
        private Database Database { get; }

        private SessionService SessionService { get; }

        public PostService(Database database, SessionService sessionService)
        {
            this.Database = database;
            this.SessionService = sessionService;
        }

        /// <summary>
        /// Get post from the database with id.
        /// </summary>
        public async Task<PostModel> GetPostById(int id)
        {
            var postPoco = await this.Database.QueryOne<PostPoco>("SELECT * FROM posts p WHERE p.post_id=@postId;", new NpgsqlParameter("postId", id));

            if (postPoco == null)
            {
                return null;
            }

            var post = new PostModel
            {
                Content = postPoco.Content,
                Id = postPoco.PostId,
                Title = postPoco.Title
            };

            var tagsPoco = await this.GetPostTags(postPoco.PostId);

            var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

            post.Tags = tempTags;

            return post;
        }

        /// <summary>
        /// Retrieves the latest posts.
        /// </summary>
        public async Task<PostModel[]> GetLatestPosts(int count)
        {
            var postPocos = await this.Database.Query<PostPoco>("SELECT * FROM posts p ORDER BY p.post_id LIMIT @count;", new NpgsqlParameter("count", count));
            var posts = new List<PostModel>();

            foreach (var postPoco in postPocos)
            {
                var post = new PostModel
                {
                    Content = postPoco.Content,
                    Id = postPoco.PostId,
                    Title = postPoco.Title
                };

                var tagsPoco = await this.GetPostTags(postPoco.PostId);

                var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

                post.Tags = tempTags;

                posts.Add(post);
            }

            return posts.ToArray();
        }

        /// <summary>
        /// Updates post with the new values
        /// </summary>
        public async Task UpdatePost(PostModel post)
        {
            var session = this.SessionService.Session;
            var postPoco = new PostPoco
            {
                Content = post.Content,
                Title = post.Title,
                UserId = session.UserAccount.UserId,
                PostId = post.Id
            };

            await this.DeletePostTags(post.Id);

            await this.AddPostTags(post.Tags, post.Id);

            await this.Database.Update(postPoco);
        }

        /// <summary>
        /// Retrieves tags from post id.
        /// </summary>
        private async Task<TagPoco[]> GetPostTags(int id)
        {
            var tags = await this.Database.Query<TagPoco>(
                "SELECT * FROM posts_tags pt INNER JOIN tags tag ON pt.tag_id = tag.tag_id WHERE pt.post_id = @postId ORDER BY pt.position;",
                new NpgsqlParameter("postId", id));

            return tags.ToArray();
        }

        /// <summary>
        /// Retrieves all posts from the database.
        /// </summary>
        public async Task<PostModel[]> GetAllPosts()
        {
            var postPocos = await this.Database.Query<PostPoco>("SELECT * FROM posts p ORDER BY p.post_id;");
            var posts = new List<PostModel>();

            foreach (var postPoco in postPocos)
            {
                var post = new PostModel
                {
                    Content = postPoco.Content,
                    Id = postPoco.PostId,
                    Title = postPoco.Title
                };

                var tagsPoco = await this.GetPostTags(postPoco.PostId);

                var tempTags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

                post.Tags = tempTags;

                posts.Add(post);
            }

            return posts.ToArray();
        }

        /// <summary>
        /// Creates post.
        /// </summary>
        public async Task<int> CreatePost(FormPostModel postModel)
        {
            var session = this.SessionService.Session;
            var postPoco = new PostPoco
            {
                Content = postModel.Content,
                Title = postModel.Title,
                UserId = session.UserAccount.UserId
            };

            int postId = await this.Database.Insert(postPoco);

            await this.AddPostTags(postModel.Tags, postId);

            return postId;
        }

        private Task AddPostTags(string tagsInLine, int postId)
        {
            return this.AddPostTags(tagsInLine.Split(','), postId);
        }

        /// <summary>
        /// Links tag to the post if the tag doesn't exist creates one
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="postId"></param>
        /// <returns></returns>
        private async Task AddPostTags(string[] tags, int postId)
        {
            for (var i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                var tagPoco = await this.Database.QueryOne<TagPoco>("SELECT * FROM tags t WHERE t.tag_name = @tagName;",
                    new NpgsqlParameter("tagName", tag));
                if (tagPoco == null)
                {
                    tagPoco = new TagPoco
                    {
                        TagName = tag

                    };

                    tagPoco.TagId = await this.Database.Insert(tagPoco);
                }

                var postsTagsPoco = new PostsTagsPoco
                {
                    PostId = postId,
                    TagId = tagPoco.TagId,
                    Position = i
                    
                };

                await this.Database.Insert(postsTagsPoco);
            }
        }

        private async Task DeletePostTags(int postId)
        {
            var postsTagsPocos = await this.Database.Query<PostsTagsPoco>("SELECT * FROM posts_tags pt WHERE pt.post_id = @postId;",
                new NpgsqlParameter("postId", postId));

            foreach (var postsTagsPoco in postsTagsPocos)
            {
                await this.Database.Delete(postsTagsPoco);
            }
        }

        public async Task DeletePost(int id)
        {
            var postTags = await this.Database.Query<PostsTagsPoco>(
                "SELECT * FROM posts_tags pt WHERE pt.post_id = @postId;", new NpgsqlParameter("postId", id));

            foreach (var postsTagsPoco in postTags)
            {
                await this.Database.Delete(postsTagsPoco);
            }

            var postPoco = await this.Database.QueryOne<PostPoco>("SELECT * FROM posts p WHERE p.post_id = @postId;",
                new NpgsqlParameter("postId", id));
            await this.Database.Delete(postPoco);
        }
    }
}