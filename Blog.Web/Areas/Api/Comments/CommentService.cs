using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Users;
using Blog.Web.DAL;
using Npgsql;

namespace Blog.Web.Areas.Api.Comments
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CommentService
    {
        private Database Database { get; }
        private UserService UserService { get; }

        public CommentService(Database database, UserService userService)
        {
            this.Database = database;
            this.UserService = userService;
        }

        private async Task<CommentModel> ConvertCommentPocoToCommentModel(CommentPoco commentPoco) =>
            new()
            {
                CommentId = commentPoco.CommentId,
                Content = commentPoco.Content,
                User = await this.UserService.GetUserById(commentPoco.UserId),
                CreatedOn = commentPoco.CreatedOn,
                Edited = commentPoco.Edited
            };

        private async Task<CommentModel[]> ConvertCommentPocosToCommentModels(CommentPoco[] commentPocos)
        {
            var commentModels = new List<CommentModel>();

            foreach (var commentPoco in commentPocos)
            {
                commentModels.Add(await this.ConvertCommentPocoToCommentModel(commentPoco));
            }

            return commentModels.ToArray();
        }

        public async Task<CommentModel[]> GetCommentsWithPostId(int postId, int offset = 0, int limit = 10)
        {
            var commentPocos = await this.Database.Query<CommentPoco>(
                "SELECT * FROM comments WHERE post_id = @postId AND parent_id IS NULL ORDER BY created_on DESC OFFSET @offset LIMIT @limit;",
                new NpgsqlParameter("postId", postId),
                new NpgsqlParameter("offset", offset),
                new NpgsqlParameter("limit", limit)
            );

            var commentModels = await this.ConvertCommentPocosToCommentModels(commentPocos.ToArray());

            foreach (var commentModel in commentModels)
            {
                var childrenCommentPocos = await this.Database.Query<CommentPoco>(
                    "SELECT * FROM comments WHERE parent_id = @parentId ORDER BY created_on ASC OFFSET @offset LIMIT @limit;",
                    new NpgsqlParameter("parentId", commentModel.CommentId),
                    new NpgsqlParameter("offset", offset),
                    new NpgsqlParameter("limit", limit)
                );

                commentModel.Children = await this.ConvertCommentPocosToCommentModels(childrenCommentPocos.ToArray());
            }

            return commentModels;
        }

        public async Task<int?> CreateComment(CommentPoco commentPoco) => await this.Database.Insert(commentPoco);

        public async Task UpdateComment(CommentPoco commentPoco) => await this.Database.Update(commentPoco);

        public async Task DeleteComment(CommentPoco commentPoco) => await this.Database.Delete(commentPoco);

        public async Task<CommentPoco> GetCommentById(int commentId)
        {
            var commentPoco = await this.Database.QueryOne<CommentPoco>(
                "SELECT * FROM comments WHERE comment_id=@commentId;",
                new NpgsqlParameter("commentId", commentId)
                );

            return commentPoco;
        }
    }
}
