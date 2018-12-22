using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Models
{
    public class PostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public Comment[] Comments { get; set; }
    }

    public class Comment
    {
        public string Content { get; set; }
        public string DateCreated { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }

    }
}
