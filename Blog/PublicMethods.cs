using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    public class Run
    {
        public static void Command(string command)
        {
            using (var conn = new NpgsqlConnection("Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;"))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(command, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine(rdr["name"]);
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }
    }
}
