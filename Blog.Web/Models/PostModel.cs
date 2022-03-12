using System;
using Blog.Web.Areas.Admin.Posts;
using Blog.Web.DAL;

namespace Blog.Web.Models
{
    public class PostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
        public TagModel[] Tags { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public PostUserModel Author { get; set; }

        public static PostModel FromPoco(PostPoco poco) => new()
        {
            Title = poco.Title,
            Content = poco.Content,
            Id = poco.PostId,
            CreatedOn = poco.CreatedOn,
            UpdatedOn = poco.UpdatedOn
        };
    }
}
