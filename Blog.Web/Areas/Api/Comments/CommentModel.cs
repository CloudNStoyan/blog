using Blog.Web.DAL;

namespace Blog.Web.Areas.Api.Comments
{
    public class CommentModel
    {
        public UserPoco User { get; set; }
        public string Content { get; set; }
        public CommentModel Parent { get; set; }
    }
}
