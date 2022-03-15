using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Users;
using Blog.Web.DAL;

namespace Blog.Web.Areas.Admin.Posts
{
    public class CommentService
    {
        private Database Database { get; }

        public CommentService(Database database)
        {
            this.Database = database;
        }

        public async Task CreateComment(string content, int userId, int postId)
        {
            var commentPoco = new CommentPoco
            {
                Content = content,
                UserId = userId,
                PostId = postId
            };

            await this.Database.Insert(commentPoco);
        }
    }
}
