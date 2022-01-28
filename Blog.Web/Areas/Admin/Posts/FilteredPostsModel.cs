using Blog.Web.Models;

namespace Blog.Web.Areas.Admin.Posts
{
    public class FilteredPostsModel
    {
        public PostModel[] Posts { get; set; }
        public PostFilter Filter { get; set; }
    }
}
