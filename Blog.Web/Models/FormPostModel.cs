namespace Blog.Web.Models
{
    public class FormPostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
    }

    public class FormEditModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public int Id { get; set; }
    }
}
