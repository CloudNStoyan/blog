using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;
using NpgsqlTypes;

namespace Blog.Web.Areas.Admin.Posts
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

        private async Task<PostModel> ConvertPostPocoToPostModel(PostPoco postPoco)
        {
            var model = PostModel.FromPoco(postPoco);

            var tagsPoco = await this.GetPostTags(postPoco.PostId);

            model.Tags = tagsPoco.Select(tagPoco => tagPoco.TagName).ToArray();

            return model;
        }

        private async Task<PostModel[]> ConvertPostPocoToPostModel(PostPoco[] postPocos)
        {
            var models = new List<PostModel>();

            foreach (var postPoco in postPocos)
            {
                models.Add(await this.ConvertPostPocoToPostModel(postPoco));
            }

            return models.ToArray();
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

            return await this.ConvertPostPocoToPostModel(postPoco);
        }

        public async Task<FilteredPostsModel> GetPosts(PostFilter filter)
        {
            filter ??= new PostFilter();

            var postgresFilters = new List<string>();
            var postgresParameters = new List<NpgsqlParameter>();

            string[] tsQueryValues = EscapeInputForTsQuery(filter.Search);

            if (tsQueryValues is { Length: > 0 })
            {
                var tsQuery = NpgsqlTsQuery.Parse(string.Join('&', tsQueryValues.Select(term => $"{term.ToLower()}:*")));

                postgresFilters.Add("search_vector @@ @postQuery");
                postgresParameters.Add(new NpgsqlParameter
                {
                    DataTypeName = "tsquery",
                    ParameterName = "postQuery",
                    Value = tsQuery
                });
            }

            if (filter.UserId > 0)
            {
                postgresFilters.Add("user_id = @userId");
                postgresParameters.Add(new NpgsqlParameter("userId", filter.UserId));
            }

            string orderBy = PostFilter.PostFilterOrderByToSqlColumn(filter.OrderBy);

            string sort = PostFilter.PostFilterSortToSql(filter.Sort);

            string filters = postgresFilters.Count > 0
                ? $"WHERE {string.Join(" AND ", postgresFilters)}"
                : string.Empty;

            string offsetSql = string.Empty;

            if (filter.Offset > 0)
            {
                offsetSql = "OFFSET @offset";
                postgresParameters.Add(new NpgsqlParameter("offset", filter.Offset));
            }

            string limitSql = string.Empty;

            if (filter.Limit > 1)
            {
                limitSql = "LIMIT @limit";
                postgresParameters.Add(new NpgsqlParameter("limit", filter.Limit));
            }

            string sql = 
                $"SELECT * FROM posts {filters} ORDER BY {orderBy} {sort} {offsetSql} {limitSql};";

            var filteredPosts = await this.Database.Query<PostPoco>(sql, postgresParameters.ToArray());

            return new FilteredPostsModel
            {
                Posts = await this.ConvertPostPocoToPostModel(filteredPosts.ToArray()),
                Filter = filter
            };
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
                PostId = post.Id,
                SearchVector = NpgsqlTsVector.Parse(post.Title + " " + string.Join(',', post.Tags)),
                CreatedOn = post.CreatedOn,
                UpdatedOn = post.UpdatedOn
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
        /// Creates post.
        /// </summary>
        public async Task<int> CreatePost(FormPostModel model)
        {
            var session = this.SessionService.Session;
            var postPoco = new PostPoco
            {
                Content = model.Content,
                Title = model.Title,
                UserId = session.UserAccount.UserId,
                SearchVector = NpgsqlTsVector.Parse(model.Title + " " + model.Tags),
                CreatedOn = model.CreatedOn,
                UpdatedOn = model.UpdatedOn
            };

            int? postId = await this.Database.Insert(postPoco);

            await this.AddPostTags(model.Tags, postId!.Value);

            return postId.Value;
        }

        private Task AddPostTags(string tagsInLine, int postId) => this.AddPostTags(tagsInLine.Split(','), postId);

        /// <summary>
        /// Links tag to the post if the tag doesn't exist creates one
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="postId"></param>
        /// <returns></returns>
        private async Task AddPostTags(string[] tags, int postId)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                string tag = tags[i].Trim();

                var tagPoco = await this.Database.QueryOne<TagPoco>("SELECT * FROM tags t WHERE t.tag_name = @tagName;",
                    new NpgsqlParameter("tagName", tag));

                if (tagPoco == null)
                {
                    tagPoco = new TagPoco
                    {
                        TagName = tag

                    };

                    tagPoco.TagId = (await this.Database.Insert(tagPoco))!.Value;
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

        /// <summary>
        /// Escapes and prepares user input to be used in a NpgsqlTsQuery by removing every
        /// character that is not a letter or a number
        /// </summary>
        /// <param name="input">User's raw input</param>
        /// <returns>Escaped and prepared for NpgsqlTsQuery array of words</returns>
        private static string[] EscapeInputForTsQuery(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            // We get only the words (which are separated by whitespace, remove the empty lines and trim the words)
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word.Trim()).ToArray();

            // And then we remove every non-digit and non-letter character in
            // a word to prevent the user to interfer with the tsquery
            words = words.Select(word => Regex.Replace(word, "[^A-ZА-Яа-яa-z0-9]", "")).Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();

            return words;
        }
    }
}