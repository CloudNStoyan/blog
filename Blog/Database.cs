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
        private static string ConnectionString =
            @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";

        public static void StartTesting()
        {
            var a = Database.Query<User>(@"SELECT * FROM users;");
            foreach (var user in a)
            {
                Console.WriteLine(user.Name);   
            }
        }

        public static List<T> Query<T>(string sql)
        {
            List<T> result = new List<T>();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var args = new object[rdr.FieldCount];

                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                Type sqlType = rdr[i].GetType();

                                var properties = typeof(T).GetProperties();

                                for (int j = 0; j < properties.Length; j++)
                                {
                                    var property = properties[j];
                                    if (property.PropertyType == sqlType && property.Name.ToLowerInvariant().Trim() == rdr.GetName(i).ToLowerInvariant().Trim())
                                    {
                                        args[j] = rdr[i];
                                        break;
                                    }
                                }
                            }
                            var instance = (T) Activator.CreateInstance(typeof(T),args);

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
        public int User_id { get; private set; }

        public string Name { get; private set; }

        public string Password { get; private set; }

        public User(int user_id,string name,string password)
        {
            this.User_id = user_id;
            this.Name = name;
            this.Password = password;
        }
    }
}
