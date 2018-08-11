using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace Blog
{
    /// <summary>
    /// Database class to easier your communicating between your database and C# using Npgsql.
    /// </summary>
    public class Database
    {
        private NpgsqlConnection Connection { get; }

        /// <summary>
        /// <paramref name="conn"/>: The NpgsqlConnection that the database class uses.
        /// </summary>
        /// <param name="conn"></param>
        public Database(NpgsqlConnection conn)
        {
            this.Connection = conn;
        }

        /// <summary>
        /// SQL Query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns><paramref name="T"/> list.</returns>
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
                    var properties = instanceType.GetProperties().ToDictionary(x => x.GetCustomAttribute<ColumnAttribute>(), x => x);

                    while (reader.Read())
                    {
                        var instance = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var returnedElement = reader[i];

                            if (returnedElement is DBNull)
                            {
                                returnedElement = null;
                            }

                            string propertyName = reader.GetName(i);

                            if (properties.Count(p => p.Key.Name == propertyName) > 0)
                            {
                                var property = properties.First(p => p.Key.Name == propertyName).Value;
                                var propertyType = property?.PropertyType;
                                var sqlColumnType = returnedElement?.GetType();

                                if (propertyType != sqlColumnType)
                                {
                                    throw new Exception($"Property type and sql return type are not the same! Property Type {propertyType}, Sql Return Type {sqlColumnType}");
                                }

                                property?.SetValue(instance, returnedElement);
                            }
                        }

                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// SQL Query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        public List<T> Query<T>(string sql, Dictionary<string, object> parametars) where T : class, new()
        {
            return this.Query<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// SQL Query for one row.
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>Single row in type <typeparamref name="T"/>.</returns>
        public T QueryOne<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

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

        /// <summary>
        /// SQL Query for one row.
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>Single row in type <typeparamref name="T"/>.</returns>
        public T QueryOne<T>(string sql, Dictionary<string, object> parametars) where T : class,new()
        {
            return this.QueryOne<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// Get one row from a query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>One element of type <paramref name="T"/>.</returns>
        public T Execute<T>(string sql, params NpgsqlParameter[] parametars)
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

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

                        if (returnedElement is DBNull)
                        {
                            returnedElement = null;
                        }

                        if (reader.FieldCount > 1)
                        {
                            throw new Exception($"There are more than one collumn! ColumnAttribute returned: {reader.FieldCount}");
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

        /// <summary>
        /// Get one row from a query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>One element of type <paramref name="T"/>.</returns>
        public T Execute<T>(string sql, Dictionary<string, object> parametars)
        {
            return this.Execute<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// Execute something without query.
        /// </summary>
        /// <returns>The number of columns that are changed</returns>
        public int ExecuteNonQuery(string sql, params NpgsqlParameter[] parametars)
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars);

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute something without returning a result.
        /// </summary>
        /// <returns>The number of rows that are changed.</returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parametars)
        {
            return this.ExecuteNonQuery(sql, ConvertDictionaryToParametars(parametars));
        }

        public int Insert<T>(T poco) where T : class,new ()
        {
            if (typeof(T).GetCustomAttribute<TableAttribute>() == null)
            {
                throw new Exception($"This class must have table attribute! class: '{typeof(T)}'");
            }

            string table = poco.GetType().GetCustomAttribute<TableAttribute>().Name;
            var columns = poco.GetType().GetProperties().ToDictionary(x => x.GetCustomAttribute<ColumnAttribute>().Name).ToList();
            var values = poco.GetType().GetProperties().ToDictionary(x => x.GetValue(poco,null)).ToList();

            using (var command = new NpgsqlCommand($"INSERT INTO {table} ({String.Join(",", columns)}) VALUES ({String.Join(",", values)})",this.Connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        private static NpgsqlParameter[] ConvertDictionaryToParametars(Dictionary<string, object> dic)
        {
            var listOfParametars = new List<NpgsqlParameter>();

            foreach (var pair in dic)
            {
                listOfParametars.Add(new NpgsqlParameter(pair.Key, pair.Value));
            }

            return listOfParametars.ToArray();
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

        private static string ConvertConventionForProperty(string variableName)
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
}
