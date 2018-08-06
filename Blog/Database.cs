using System;
using System.Collections.Generic;
using System.Reflection;
using Npgsql;

namespace Blog
{
    public class Database
    {
        private NpgsqlConnection Connection { get; }

        public Database(NpgsqlConnection conn)
        {
            this.Connection = conn;
        }

        public List<T> Query<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {
            var result = new List<T>();

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                foreach (var parametar in parametars)
                {
                    command.Parameters.Add(parametar);
                }

                var instanceType = typeof(T);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var instance = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            instanceType.GetProperty(reader.GetName(i))?.SetValue(instance, reader[i]);
                        }

                        result.Add(instance);
                    }
                }
            }

            return result;
        }
    }


    public class User
    {
        public int user_id { get; private set; }

        public string name { get; private set; }

        public string password { get; private set; }

        public User()
        {

        }
    }
}
