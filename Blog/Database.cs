using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    public static class Database
    {
        private static string DbPath =
            @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";

        public static void StartTesting()
        {
            var a = Database.Query<User>("SELECT 1;");
            Console.WriteLine(a[0].Name);
        }

        public static List<T> Query<T>(string sql)
        {
            List<T> result = new List<T>();

            using (var conn = new NpgsqlConnection(DbPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var instance = (T) Activator.CreateInstance(typeof(T),"Dwarf","2");

                            result.Add(instance);
                        }
                    }
                }
                conn.Close();
            }

            return result;
        }
    }

    public class User
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        public User(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}
