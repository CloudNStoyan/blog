using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql;

namespace Blog.Web.DAL
{
    public class Database
    {
        private NpgsqlConnection Connection { get; }

        /// <summary>
        /// <paramref name="connection"/>: The NpgsqlConnection that the database class uses.
        /// </summary>
        public Database(NpgsqlConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// SQL Query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns><paramref name="T"/> list.</returns>
        public async Task<List<T>> Query<T>(string sql, params NpgsqlParameter[] parametars) where T : new()
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            await this.OpenConnectionIfNeeded();

            var result = new List<T>();
            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.AddRange(parametars);

            await using var reader = await command.ExecuteReaderAsync();
            var instanceType = typeof(T);
            var properties = instanceType.GetProperties().ToDictionary(x => x.GetCustomAttribute<ColumnAttribute>(), x => x);

            while (await reader.ReadAsync())
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

                    // If our Poco doesn't have any properties that match the DB column name we skip
                    if (properties.All(p => p.Key?.Name != propertyName))
                    {
                        continue;
                    }

                    var property = properties.First(p => p.Key?.Name == propertyName).Value;
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

                result.Add(instance);
            }

            return result;
        }

        private static bool IsNullable(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);

        /// <summary>
        /// SQL Query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        public Task<List<T>> Query<T>(string sql, Dictionary<string, object> parametars) where T :  new()
        {
            return this.Query<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// SQL Query for one row.
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>Single row in type <typeparamref name="T"/>.</returns>
        public async Task<T> QueryOne<T>(string sql, params NpgsqlParameter[] parametars) where T : class, new()
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            var queryResult = await this.Query<T>(sql, parametars);

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
        public Task<T> QueryOne<T>(string sql, Dictionary<string, object> parametars) where T : class, new()
        {
            return this.QueryOne<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// Get one row from a query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        /// <returns>One element of type <paramref name="T"/>.</returns>
        public async Task<T> Execute<T>(string sql, params NpgsqlParameter[] parametars)
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            await this.OpenConnectionIfNeeded();

            T result = default;

            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.AddRange(parametars);

            await using var reader = await command.ExecuteReaderAsync();
            bool valueWasExtracted = false;

            while (await reader.ReadAsync())
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

            return result;
        }

        /// <summary>
        /// Get one row from a query
        /// </summary>
        /// <typeparam name="T">Class that will be filled with the result from the query.</typeparam>
        public Task<T> Execute<T>(string sql, Dictionary<string, object> parametars)
        {
            return this.Execute<T>(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// Execute something without query.
        /// </summary>
        /// <returns>The number of columns that are changed</returns>
        public async Task<int> ExecuteNonQuery(string sql, params NpgsqlParameter[] parametars)
        {
            ThrowIfSqlOrParamsAreNull(sql, parametars);

            await this.OpenConnectionIfNeeded();

            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.AddRange(parametars);

            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Execute something without returning a result.
        /// </summary>
        /// <returns>The number of rows that are changed.</returns>
        public Task<int> ExecuteNonQuery(string sql, Dictionary<string, object> parametars)
        {
            return this.ExecuteNonQuery(sql, ConvertDictionaryToParametars(parametars));
        }

        /// <summary>
        /// Insert something with poco class to the database.
        /// </summary>
        /// <returns>The primary key of the created poco.</returns>
        public async Task<int?> Insert<T>(T poco) where T : new()
        {
            await this.OpenConnectionIfNeeded();

            var pocoType = typeof(T);

            var tableAttribute = pocoType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new Exception($"This class must have table attribute! class: '{pocoType}'");
            }

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() ?? throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            var columns = columnAttributes.Where(c => !c.IsPrimaryKey).Select(c => c.Name).ToList();

            var values = properties.Where(x => !x.GetCustomAttribute<ColumnAttribute>()!.IsPrimaryKey)
                .Select(x => x.GetValue(poco, null) ?? DBNull.Value).ToList();

            var parametars = values.Select((x, i) => new NpgsqlParameter("p" + i, x)).ToArray();

            var parametarNames = parametars.Select(x => "@" + x.ParameterName).ToArray();

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

            if (primaryKeyColumnName == null)
            {
                throw new Exception("There wasn't any primary key found with in property attributes");
            }

            string sql = $"INSERT INTO \"{tableAttribute.Schema}\".\"{tableAttribute.Name}\" ({string.Join(",", columns)}) VALUES ({string.Join(",", parametarNames)}) RETURNING {primaryKeyColumnName};";

            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.AddRange(parametars.ToArray());
            return (int?) await command.ExecuteScalarAsync();
        }

        /// <summary>
        /// Update something in the database using poco class.
        /// </summary>
        public async Task Update<T>(T poco)
        {
            await this.OpenConnectionIfNeeded();

            var pocoType = typeof(T);

            var tableAttribute = pocoType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new Exception($"This class must have table attribute! class: '{pocoType}'");
            }

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() ?? throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            string table = tableAttribute.Name;
            string schema = tableAttribute.Schema;

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

            if (primaryKeyColumnName == null)
            {
                throw new Exception($"Poco of type '{nameof(pocoType)}' doesnt have Primary Key property");
            }

            var columnsAndValuesDic = properties.Where(x => !x.GetCustomAttribute<ColumnAttribute>()!.IsPrimaryKey)
                .ToDictionary(x => x.GetCustomAttribute<ColumnAttribute>()!.Name, x => x.GetValue(poco, null) ?? DBNull.Value);

            var columnsAndValues = columnsAndValuesDic.Select((x, i) => $"{x.Key}=@a{i}");

            var parametars = columnsAndValuesDic.Select((x, i) => new NpgsqlParameter($"a{i}", x.Value)).ToList();

            var primaryKeyColumnValue = properties
                .FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()!.IsPrimaryKey)
                ?.GetValue(poco, null) ?? DBNull.Value;

            parametars.Add(new NpgsqlParameter("i", primaryKeyColumnValue));

            string sql = $"UPDATE \"{schema}\".\"{table}\" SET {string.Join(",", columnsAndValues)} WHERE {primaryKeyColumnName}=@i;";

            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.AddRange(parametars.ToArray());
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes something from the database using poco class.
        /// </summary>
        /// <returns>How many rows are changed</returns>
        public async Task<int> Delete<T>(T poco)
        {
            await this.OpenConnectionIfNeeded();

            var pocoType = typeof(T);

            var properties = pocoType.GetProperties();

            var columnAttributes = properties.Select(x =>
                x.GetCustomAttribute<ColumnAttribute>() ??
                throw new Exception($"Property doesnt have attribute '{nameof(ColumnAttribute)}'")).ToList();

            var tableAttribute = pocoType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new Exception($"This class must have table attribute! class: '{pocoType}'");
            }

            var primaryKey = properties.FirstOrDefault(x => x.GetCustomAttribute<ColumnAttribute>()!.IsPrimaryKey);

            if (primaryKey == null)
            {
                throw new Exception($"Poco of type '{nameof(pocoType)}' doesnt have Primary Key property");
            }

            string primaryKeyColumnName = columnAttributes.FirstOrDefault(x => x.IsPrimaryKey)?.Name;

            var parametar = new NpgsqlParameter("i", primaryKey.GetValue(poco, null));

            string sql = $"DELETE FROM \"{tableAttribute.Schema}\".\"{tableAttribute.Name}\" WHERE {primaryKeyColumnName}=@i;";

            await using var command = new NpgsqlCommand(sql, this.Connection);
            command.Parameters.Add(parametar);
            return await command.ExecuteNonQueryAsync();
        }

        private Task OpenConnectionIfNeeded()
        {
            return this.Connection.State == ConnectionState.Closed ? this.Connection.OpenAsync() : Task.CompletedTask;
        }

        private static NpgsqlParameter[] ConvertDictionaryToParametars(Dictionary<string, object> dictionary)
        {
            return dictionary.Select(pair => new NpgsqlParameter(pair.Key, pair.Value)).ToArray();
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

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    throw new Exception($"An element of the `params` array {parameters} is null. " +
                                        $"Element index: {i}.");
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey = false;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; }
    }
}