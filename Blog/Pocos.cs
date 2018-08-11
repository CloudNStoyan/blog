namespace Blog
{
    [Table(Name = "users")]
    public class UserPoco
    {
        [Column(Name = "user_id")]
        public int UserId { get; set; }

        [Column(Name = "name")]
        public string Name { get; set; }

        [Column(Name = "password")]
        public string Password { get; set; }
    }

    [Table(Name = "posts")]
    public class PostPoco
    {
        [Column(Name = "post_id")]
        public int PostId { get; set; }

        [Column(Name = "title")]
        public string Title { get; set; }

        [Column(Name = "content")]
        public string Content { get; set; }

        [Column(Name = "user_id")]
        public int UserId { get; set; }
    }

    [Table(Name = "tags")]
    public class TagPoco
    {
        [Column(Name = "tag_id")]
        public int TagId { get; set; }

        [Column(Name = "name")]
        public string Name { get; set; }
    }

    [Table(Name = "comments")]
    public class CommentPoco
    {
        [Column(Name = "comment_id")]
        public int CommentId { get; set; }

        [Column(Name = "author_name")]
        public string AuthorName { get; set; }

        [Column(Name = "content")]
        public string Content { get; set; }

        [Column(Name = "post_id")]
        public int PostId { get; set; }

        [Column(Name = "user_id")]
        public int UserId { get; set; }
    }

    [Table(Name = "posts_tags")]
    public class PostsTagsPoco
    {
        [Column(Name = "posts_tags_id")]
        public int PostsTagsId { get; set; }

        [Column(Name = "post_id")]
        public int PostId { get; set; }

        [Column(Name = "tag_id")]
        public int TagId { get; set; }
    }
}
