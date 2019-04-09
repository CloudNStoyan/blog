using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace Blog.Web.DAL
{
    public class Database
    {
        private NpgsqlConnection Connection { get; }

        /// <summary>
        /// <paramref name="conn"/>: The NpgsqlConnection that the database class uses.
        /// </summary>
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
            this.OpenConnectionIfNot();

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

                            var sqlColumnType = reader.GetFieldType(i);

                            string propertyName = reader.GetName(i);

                            if (properties.Count(p => p.Key.Name == propertyName) > 0)
                            {
                                var property = properties.First(p => p.Key.Name == propertyName).Value;
                                var propertyType = property?.PropertyType;

                                var expectedType = IsNullable(propertyType)
                                    ? Nullable.GetUnderlyingType(propertyType)
                                    : propertyType;

                                if (expectedType != sqlColumnType)
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

        private static bool IsNullable(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);

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
        public T QueryOne<T>(string sql, Dictionary<string, object> parametars) where T : class, new()
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
            this.OpenConnectionIfNot();

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
            this.OpenConnectionIfNot();

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

        /// <summary>
        /// Insert something with poco class to the database.
        /// </summary>
        /// <returns>How many rows are changed.</returns>a
        public int Insert<T>(T poco) where T : class, new()
        {
            this.OpenConnectionIfNot();
            var pocoType = typeof(T);

            var tableAttribute = pocoType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new Exception($"This class must have table attribute! class: '{pocoType}'");
            }

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() != null
                    ? x.GetCustomAttribute<ColumnAttribute>()
                    : throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            var columns = columnAttributes.Where(c => !c.IsPrimaryKey).Select(c => c.Name).ToList();

            var values = properties.Where(x => !x.GetCustomAttribute<ColumnAttribute>().IsPrimaryKey)
                .Select(x => x.GetValue(poco, null) ?? DBNull.Value).ToList();

            var parametars = values.Select((x, i) => new NpgsqlParameter("p" + i, x)).ToArray();

            var parametarNames = parametars.Select(x => "@" + x.ParameterName).ToArray();

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

            if (primaryKeyColumnName == null)
            {
                throw new Exception("There wasn't any primary key found with in property attributes");
            }

            string sql = $"INSERT INTO \"{tableAttribute.Schema}\".\"{tableAttribute.Name}\" ({string.Join(",", columns)}) VALUES ({string.Join(",", parametarNames)}) RETURNING {primaryKeyColumnName};";

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars.ToArray());
                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Update something in the database using poco class.
        /// </summary>
        public void Update<T>(T poco)
        {
            this.OpenConnectionIfNot();
            var pocoType = typeof(T);

            var tableAttribute = pocoType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new Exception($"This class must have table attribute! class: '{pocoType}'");
            }

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() != null
                    ? x.GetCustomAttribute<ColumnAttribute>()
                    : throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            string table = tableAttribute.Name;
            string schema = tableAttribute.Schema;

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

            var columnsAndValuesDic = properties.Where(x => !x.GetCustomAttribute<ColumnAttribute>().IsPrimaryKey)
                .ToDictionary(x => x.GetCustomAttribute<ColumnAttribute>().Name, x => x.GetValue(poco, null) ?? DBNull.Value);

            var columnsAndValues = columnsAndValuesDic.Select((x, i) => $"{x.Key}=@a{i}");

            var parametars = columnsAndValuesDic.Select((x, i) => new NpgsqlParameter($"a{i}", x.Value)).ToList();

            var primaryKeyColumnValue = properties
                .FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>().IsPrimaryKey)
                ?.GetValue(poco, null) ?? DBNull.Value;

            parametars.Add(new NpgsqlParameter("i", primaryKeyColumnValue));

            string sql = $"UPDATE \"{schema}\".\"{table}\" SET {string.Join(",", columnsAndValues)} WHERE {primaryKeyColumnName}=@i;";

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.AddRange(parametars.ToArray());
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes something from the database using poco class.
        /// </summary>
        /// <returns>How many rows are changed</returns>
        public int Delete<T>(T poco)
        {
            this.OpenConnectionIfNot();
            var pocoType = typeof(T);

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() ??
                throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            var tableAttributes = pocoType.GetCustomAttribute<TableAttribute>();

            var primaryKey = properties.FirstOrDefault(x => x.GetCustomAttribute<ColumnAttribute>().IsPrimaryKey);

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(x => x.IsPrimaryKey)?.Name;

            var parametar = new NpgsqlParameter("i", primaryKey.GetValue(poco, null));

            string sql = $"DELETE FROM \"{tableAttributes.Schema}\".\"{tableAttributes.Name}\" WHERE {primaryKeyColumnName}=@i;";

            using (var command = new NpgsqlCommand(sql, this.Connection))
            {
                command.Parameters.Add(parametar);
                return command.ExecuteNonQuery();
            }
        }

        private void OpenConnectionIfNot()
        {
            if (this.Connection != null && this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
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
    }
}