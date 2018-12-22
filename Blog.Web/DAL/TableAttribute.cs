using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.DAL
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; }
    }
}
