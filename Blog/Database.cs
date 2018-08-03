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

        public List<T> Query<T>(string sql, params object[] parametars) where T : class , new ()
        {
            var dbResult = new List<T>();

            this.Connection.Open();

            var paramatarNames = this.GetParametarNames(sql);

            using (var cmd = new NpgsqlCommand(sql, this.Connection))
            {
                for (int i = 0; i < paramatarNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue(paramatarNames[i], parametars[i]);
                }

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var constructorArguments = new object[rdr.FieldCount];

                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            Type sqlType = rdr[i].GetType();

                            var properties = typeof(T).GetProperties();

                            for (int j = 0; j < properties.Length; j++)
                            {
                                var property = properties[j];
                                if (property.PropertyType == sqlType && property.Name.ToLowerInvariant().Trim() ==
                                    rdr.GetName(i).ToLowerInvariant().Trim())
                                {
                                    constructorArguments[j] = rdr[i];
                                    break;
                                }
                            }
                        }

                         

                        List<Type> consturctorTypes = new List<Type>();

                        foreach (object constructorArgument in constructorArguments)
                        {
                            consturctorTypes.Add(constructorArgument.GetType());
                        }

                        ConstructorInfo constructor =
                            typeof(T).GetConstructor(consturctorTypes.ToArray());

                        var instance = constructor.Invoke(constructorArguments);

                        //var instance = (T) Activator.CreateInstance(typeof(T), constructorArguments);

                        dbResult.Add((T)instance);
                    }
                }

            }

            return dbResult;
        }

        private string[] GetParametarNames(string sql)
        {
            var paramNames = new List<string>();

            string[] splitedSql = sql.Split('@');

            for (int i = 1; i < splitedSql.Length; i++)
            {
                paramNames.Add(splitedSql[i][0].ToString());
            }

            return paramNames.ToArray();
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
