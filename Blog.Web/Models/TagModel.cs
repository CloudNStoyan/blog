using Blog.Web.DAL;

namespace Blog.Web.Models
{
    public class TagModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        public static TagModel FromPoco(TagPoco tagPoco) => new() { Name = tagPoco.TagName, TagId = tagPoco.TagId };
    }
}
