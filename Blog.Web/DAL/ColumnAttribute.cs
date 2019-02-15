using System;

namespace Blog.Web.DAL
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey = false;
    }
}
