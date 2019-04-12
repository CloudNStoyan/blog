using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Services
{
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
        /// Get post from the database with id
        /// </summary>
        public async Task<PostModel> GetPostById(int id)
        {
            var postPoco = await this.Database.QueryOne<PostPoco>("SELECT * FROM posts p WHERE p.post_id=@postId;", new NpgsqlParameter("postId", id));
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
        /// Retrieves the latest posts
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
        /// Retrieves tags from post id
        /// </summary>
        private async Task<TagPoco[]> GetPostTags(int id)
        {
            var tags = await this.Database.Query<TagPoco>(
                "SELECT * FROM posts_tags pt INNER JOIN tags tag ON pt.tag_id = tag.tag_id WHERE pt.post_id = @postId;",
                new NpgsqlParameter("postId", id));

            return tags.ToArray();
        }

        /// <summary>
        /// Creates post
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>

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

        private async Task AddPostTags(string tagsInLine, int postId)
        {
            string[] tags = tagsInLine.Split(',');

            foreach (string tag in tags)
            {
                var tagPoco = await this.Database.QueryOne<TagPoco>("SELECT * FROM tags t WHERE t.tag_name = @tagName",
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
                    TagId = tagPoco.TagId
                };

                await this.Database.Insert(postsTagsPoco);
            }
        }
    }
}