using System.Threading.Tasks;
using Blog.Web.DAL;
using Npgsql;

namespace Blog.Web.Areas.Api.Comments
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CommentService
    {
        private Database Database { get; }

        public CommentService(Database database)
        {
            this.Database = database;
        }

        public async Task<int?> CreateComment(string content, int userId, int postId, int? parentId)
        {
            var commentPoco = new CommentPoco
            {
                Content = content,
                UserId = userId,
                PostId = postId,
                ParentId = parentId
            };

            return await this.Database.Insert(commentPoco);
        }

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
