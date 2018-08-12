using System;

namespace Blog
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey = false;
    }
}
