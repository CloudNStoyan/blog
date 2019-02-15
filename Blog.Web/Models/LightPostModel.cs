namespace Blog.Web.Models
{
    public class LightPostModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
    }
}
