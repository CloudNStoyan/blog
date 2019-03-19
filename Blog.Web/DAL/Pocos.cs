using System;

namespace Blog.Web.DAL
{
    [Table(Name = "users", Schema = "public")]
    public class UserPoco
    {
        [Column(Name = "user_id", IsPrimaryKey = true)]
        public int UserId { get; set; }

        [Column(Name = "username")]
        public string Name { get; set; }

        [Column(Name = "password")]
        public byte[] Password { get; set; }

        [Column(Name = "avatar_url")]
        public string AvatarUrl { get; set; }
    }

    [Table(Name = "posts", Schema = "public")]
    public class PostPoco
    {
        [Column(Name = "post_id", IsPrimaryKey = true)]
        public int PostId { get; set; }

        [Column(Name = "title")]
        public string Title { get; set; }

        [Column(Name = "content")]
        public string Content { get; set; }

        [Column(Name = "user_id")]
        public int UserId { get; set; }
    }

    [Table(Name = "tags", Schema = "public")]
    public class TagPoco
    {
        [Column(Name = "tag_id", IsPrimaryKey = true)]
        public int TagId { get; set; }

        [Column(Name = "name")]
        public string Name { get; set; }
    }

    [Table(Name = "comments", Schema = "public")]
    public class CommentPoco
    {
        [Column(Name = "comment_id", IsPrimaryKey = true)]
        public int CommentId { get; set; }

        [Column(Name = "content")]
        public string Content { get; set; }

        [Column(Name = "post_id")]
        public int PostId { get; set; }

        [Column(Name = "user_id")]
        public int UserId { get; set; }

        [Column(Name = "created_on")]
        public string CreatedOn { get; set; }
    }

    [Table(Name = "posts_tags", Schema = "public")]
    public class PostsTagsPoco
    {
        [Column(Name = "posts_tags_id", IsPrimaryKey = true)]
        public int PostsTagsId { get; set; }

        [Column(Name = "post_id")]
        public int PostId { get; set; }

        [Column(Name = "tag_id")]
        public int TagId { get; set; }
    }

    [Table(Name = "login_sessions", Schema = "public")]
    public class LoginSessions
    {
        [Column(Name = "login_sessions_id", IsPrimaryKey = true)]
        public int LoginSessionId { get; set; }
        [Column(Name = "session_key")]
        public string SessionKey { get; set; }
        [Column(Name = "login_time")]
        public DateTime LoginTime { get; set; }
        [Column(Name = "user_id")]
        public int UserId { get; set; }
    }
}
