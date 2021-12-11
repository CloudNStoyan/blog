using Blog.Web.DAL;

namespace Blog.Web.Models
{
    public class PostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
        public string[] Tags { get; set; }

        public static PostModel FromPoco(PostPoco poco) => new()
        {
            Title = poco.Title,
            Content = poco.Content,
            Id = poco.PostId
        };
    }
}
