using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Models
{
    public class FormPostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
    }
}
