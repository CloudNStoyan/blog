using System;
using System.Collections.Generic;
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
                command.Parameters.AddRange(parametars);

                var instanceType = typeof(T);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var instance = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var propertyType = instanceType.GetProperty(this.ConvertConventionForProperty(reader.GetName(i)))?.PropertyType;
                            var sqlReturnType = reader[i].GetType();

                            if (propertyType != sqlReturnType)
                            {
                                throw new Exception("Property type and sql return type are not the same!");
                            }

                            instanceType.GetProperty(this.ConvertConventionForProperty(reader.GetName(i)))?.SetValue(instance, reader[i]);
                        }

                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        public T QueryRow<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {
            T result = null;

            var queryResult = this.Query<T>(sql, parametars);

            if (queryResult.Count > 1)
            {
                throw new Exception("Returned more than one row!");
            }

            if (queryResult.Count > 0)
            {
                result = queryResult[0];
            }

            return result;
        }

        public T QueryElement<T>(string sql, params NpgsqlParameter[] parametars)
        {
            T result = default(T);
            bool filled = false;

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                foreach (var parametar in parametars)
                {
                    command.Parameters.Add(parametar);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            throw new Exception("There are more than one collumn!");
                        }

                        if (reader[0].GetType() != typeof(T))
                        {
                            throw new Exception("Given type and sql return type are not matching!");
                        }

                        if (filled)
                        {
                            throw new Exception("There is more than one value!");
                        }

                        result = (T)reader[0];
                        filled = true;
                    }
                }
            }

            return result;
        }

        public void Execute(string sql, params NpgsqlParameter[] parametars)
        {
            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);
                command.ExecuteNonQuery();
            }
        }

        private string ConvertConventionForField(string variableName)
        {
            string[] words = variableName.Split('_');
            string convertedWord = String.Empty;

            for (int i = 0; i < words.Length; i++)
            {
                if (i != 0)
                {
                    convertedWord += char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
                else
                {
                    convertedWord += words[i];
                }
            }

            return convertedWord;
        }

        private string ConvertConventionForProperty(string variableName)
        {
            string[] words = variableName.Split('_');
            string convertedWord = String.Empty;

            for (int i = 0; i < words.Length; i++)
            {
                convertedWord += char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            return convertedWord;
        }
    }


    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

    }
}
