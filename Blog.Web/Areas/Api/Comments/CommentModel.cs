using System;
using Blog.Web.DAL;

namespace Blog.Web.Areas.Api.Comments
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        public UserPoco User { get; set; }
        public string Content { get; set; }
        public CommentModel Parent { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool Edited { get; set; }
    }
}
