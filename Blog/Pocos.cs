namespace Blog
{
    public class UserPoco
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }

    public class PostPoco
    {
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }
    }

    public class TagPoco
    {
        public int TagId { get; set; }

        public string Name { get; set; }
    }

    public class CommentPoco
    {
        public int CommentId { get; set; }

        public string AuthorName { get; set; }

        public string Content { get; set; }

        public int PostId { get; set; }

        public int UserId { get; set; }
    }

    public class PostsTagsPoco
    {
        public int PostsTagsId { get; set; }

        public int PostId { get; set; }

        public int TagId { get; set; }
    }
}
