using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string AvatarUrl { get; set; }
    }
}
