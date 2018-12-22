using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.DAL
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey = false;
    }
}
