using System.Threading.Tasks;
using Blog.Web.DAL;

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

        public async Task<int?> CreateComment(string content, int userId, int postId)
        {
            var commentPoco = new CommentPoco
            {
                Content = content,
                UserId = userId,
                PostId = postId
            };

            return await this.Database.Insert(commentPoco);
        }
    }
}
