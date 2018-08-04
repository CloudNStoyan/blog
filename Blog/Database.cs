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
            var dbResult = new List<T>();

            using (var npgsqlCommand = new NpgsqlCommand(sql, this.Connection))
            {
                foreach (var parametar in parametars)
                {
                    npgsqlCommand.Parameters.Add(parametar);
                }


                using (var dataReader = npgsqlCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var instance = new T();
                        var typeOfInstance = instance.GetType();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            typeOfInstance.GetProperty(dataReader.GetName(i))?.SetValue(instance, dataReader[i]);
                        }

                        dbResult.Add(instance);
                    }
                }
            }

            return dbResult;
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
