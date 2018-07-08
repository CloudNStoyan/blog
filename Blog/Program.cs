using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    class Program
    {
        static void Main(string[] args)
        {
            Run.Command("select * from users where user_id='1'");
        }
    }
}
