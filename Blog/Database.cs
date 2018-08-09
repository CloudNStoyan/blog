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
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            var result = new List<T>();
            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                using (var reader = command.ExecuteReader())
                {
                    var instanceType = typeof(T);
                    var properties = instanceType.GetProperties().ToDictionary(x => x.Name, x => x);

                    while (reader.Read())
                    {
                        var instance = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var returnedElement = reader[i];
                            if (returnedElement.GetType().Name == "DBNull")
                            {
                                returnedElement = null;
                            }
                            string propertyName = this.ConvertConventionForProperty(reader.GetName(i));

                            var property = properties[propertyName];

                            var propertyType = property?.PropertyType;
                            var sqlColumnType = returnedElement?.GetType();

                            if (propertyType != sqlColumnType)
                            {
                                throw new Exception($"Property type and sql return type are not the same! Property Type {propertyType}, Sql Return Type {sqlColumnType}");
                            }

                            property?.SetValue(instance, returnedElement);
                        }

                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        public List<T> Query<T>(string sql, Dictionary<string, object> parametars) where T : class, new()
        {
            var listOfParametars = new List<NpgsqlParameter>();
            foreach (var pair in parametars)
            {
                listOfParametars.Add(new NpgsqlParameter(pair.Key,pair.Value));
            }

            return this.Query<T>(sql, listOfParametars.ToArray());
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

        public T QueryOne<T>(string sql, Dictionary<string, object> parametars) where T : class,new()
        {
            var listOfParametars = new List<NpgsqlParameter>();
            foreach (var pair in parametars)
            {
                listOfParametars.Add(new NpgsqlParameter(pair.Key, pair.Value));
            }

            return this.QueryOne<T>(sql, listOfParametars.ToArray());
        }

        public T Execute<T>(string sql, params NpgsqlParameter[] parametars)
        {
            T result = default;

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                using (var reader = command.ExecuteReader())
                {
                    bool valueWasExtracted = false;

                    while (reader.Read())
                    {
                        var returnedElement = reader[0];

                        if (returnedElement.GetType().Name == "DBNull")
                        {
                            returnedElement = null;
                        }

                        if (reader.FieldCount > 1)
                        {
                            throw new Exception($"There are more than one collumn! Collumn returned: {reader.FieldCount}");
                        }

                        if (returnedElement?.GetType() != typeof(T))
                        {
                            throw new Exception($"Given type and sql return type are not matching! Returned type from sql '{returnedElement?.GetType()}', given type '{typeof(T)}'");
                        }

                        if (valueWasExtracted)
                        {
                            throw new Exception("There is more than one rows!");
                        }

                        result = (T)returnedElement;
                        valueWasExtracted = true;
                    }
                }
            }

            return result;
        }

        public T Execute<T>(string sql, Dictionary<string, object> parametars)
        {
            var listOfParametars = new List<NpgsqlParameter>();
            foreach (var pair in parametars)
            {
                listOfParametars.Add(new NpgsqlParameter(pair.Key, pair.Value));
            }

            return this.Execute<T>(sql, listOfParametars.ToArray());
        }

        public int ExecuteNonQuery(string sql, params NpgsqlParameter[] parametars)
        {
            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> parametars)
        {
            var listOfParametars = new List<NpgsqlParameter>();
            foreach (var pair in parametars)
            {
                listOfParametars.Add(new NpgsqlParameter(pair.Key, pair.Value));
            }

            return this.ExecuteNonQuery(sql, listOfParametars.ToArray());
        }

        private static void ThrowIfSqlOrParamsAreNull(string sql, NpgsqlParameter[] parameters)
        {
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    throw new Exception($"An element of the `params` array {parameters} is null. " +
                                        $"Element index: {i}.");
                }
            }
        }

        private string ConvertConventionForProperty(string variableName)
        {
            string[] words = variableName.Split('_');
            string convertedWord = string.Empty;
            
            foreach (var word in words)
            {
                convertedWord += char.ToUpper(word[0]) + word.Substring(1);
            }

            return convertedWord;
        }
    }
    
    public class UserPoco
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }
}
