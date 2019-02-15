using System;

namespace Blog.Web.DAL
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; }
    }
}
