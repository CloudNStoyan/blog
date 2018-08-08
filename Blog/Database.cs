using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace Blog
{
    //Unhandled Exception: System.ArgumentException: Object of type 'System.DBNull' cannot be converted to type 'System.Int32'.

    public class Database
    {
        private NpgsqlConnection Connection { get; }

        public Database(NpgsqlConnection conn)
        {
            this.Connection = conn;
        }

        public List<T> Query<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {
            if (this.SqlOrParamsIsNull(sql, parametars))
            {
                throw new Exception("You can't pass null in the query!");
            }


            var result = new List<T>();
            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                using (var reader = command.ExecuteReader())
                {
                    var instanceType = typeof(T);
                    var properties = instanceType.GetProperties();

                    while (reader.Read())
                    {
                        var instance = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string propertyName = this.ConvertConventionForProperty(reader.GetName(i));
                            var property = properties.First(p => p.Name == propertyName);

                            var propertyType = property?.PropertyType;
                            var sqlColumnType = reader[i].GetType();

                            if (propertyType != sqlColumnType)
                            {
                                throw new Exception($"Property type and sql return type are not the same! Property Type {propertyType}, Sql Return Type {sqlColumnType}");
                            }

                            property.SetValue(instance, reader[i]);
                        }

                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        public T QueryOne<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {

            var queryResult = this.Query<T>(sql, parametars);

            if (queryResult.Count < 1)
            {
                return null;
            }

            if (queryResult.Count > 1)
            {
                throw new Exception($"Returned more than one row! Rows returned: {queryResult.Count}");
            }

            return queryResult[0];
        }

        public T Execute<T>(string sql, params NpgsqlParameter[] parametars)
        {
            T result = default(T);

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                using (var reader = command.ExecuteReader())
                {
                    bool valueWasExtracted = false;

                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            throw new Exception($"There are more than one collumn! Collumn returned: {reader.FieldCount}");
                        }

                        if (reader[0].GetType() != typeof(T))
                        {
                            throw new Exception("Given type and sql return type are not matching!");
                        }

                        if (valueWasExtracted)
                        {
                            throw new Exception($"There is more than one value!");
                        }

                        result = (T)reader[0];
                        valueWasExtracted = true;
                    }
                }
            }

            return result;
        }

        public int ExecuteNonQuery(string sql, params NpgsqlParameter[] parametars)
        {
            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                return command.ExecuteNonQuery();
            }
        }

        private bool SqlOrParamsIsNull(string sql, NpgsqlParameter[] parametars)
        {
            if (sql.Contains("null"))
            {
                return true;
            }

            foreach (var parametar in parametars)
            {
                if (parametar.IsNullable)
                {
                    return true;
                }
            }

            return false;

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
