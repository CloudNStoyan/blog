using System;

namespace Blog
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; }
    }
}
