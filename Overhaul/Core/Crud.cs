using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;

using Dapper;
using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class Crud : ICrud
    {
        private readonly IEnumerable<TableDefinition> _tableDefinitions;

        public Crud(IEnumerable<TableDefinition> tableDefinitions)
        {
            _tableDefinitions = tableDefinitions;
        }
        
        public T Create<T>(T entity) where T : class
        {
            using var conn = ConnectionManager.GetSqlConnection();
            conn.Insert(entity);
            return entity;
        }
        
        public async Task<T> CreateAsync<T>(T entity) where T : class
        {
            using var conn = ConnectionManager.GetSqlConnection();
            await conn.InsertAsync(entity);
            return entity;
        }
        
        public T GetById<T>(object id, params string[] columns) 
            where T : class
        {
            SqlConnection conn;

            if (columns.Any())
            {
                PropertyInfo keyId = GetKeyId<T>();

                if (keyId == null)
                {
                    throw new Exception($"{nameof(keyId)} is null");
                }

                var tableColumns = GetTableColumns<T>(columns);

                var query = DefaultQuery.GetById(tableColumns,
                    keyId.Name, GetTableName(typeof(T)));

                using (conn = ConnectionManager.GetSqlConnection())
                {
                    return conn.QuerySingle<T>(query, new { id });
                }
            }

            using (conn = ConnectionManager.GetSqlConnection())
            {
                return conn.Get<T>(id);
            }
        }
        
        public async Task<T> GetByIdAsync<T>(object id, params string[] columns)
            where T : class
        {
            SqlConnection conn;

            if (columns.Any())
            {
                var keyId = GetKeyId<T>();

                if (keyId == null)
                {
                    throw new Exception($"{nameof(keyId)} is null");
                }

                var tableColumns = GetTableColumns<T>(columns);

                var query = DefaultQuery.GetById(tableColumns,
                    keyId.Name, GetTableName(typeof(T)));

                using (conn = await ConnectionManager.GetSqlConnectionAsync())
                {
                    return await conn.QuerySingleAsync<T>(query, new { id })
                        .ConfigureAwait(false);
                }
            }

            using (conn = await ConnectionManager.GetSqlConnectionAsync())
            {
                return await conn.GetAsync<T>(id)
                    .ConfigureAwait(false);
            }
        }
        
        public T GetBy<T>(string columnName, object value, params string[] columns) 
            where T : class
        {
            var tableColumns = GetTableColumns<T>(columns);
            var query = DefaultQuery.GetBy(tableColumns, 
                GetTableName(typeof(T)), columnName);

            using var conn = ConnectionManager.GetSqlConnection();
            return conn.QuerySingle<T>(query, new { value });
        }
        
        public async Task<T> GetByAsync<T>(string columnName, object value, params string[] columns)
            where T : class
        {
            var tableColumns = GetTableColumns<T>(columns);
            var query = DefaultQuery.GetBy(tableColumns,
                GetTableName(typeof(T)), columnName);

            using var conn = ConnectionManager.GetSqlConnection();
            return await conn.QuerySingleAsync<T>(query, new { value });
        }

        public T Read<T>(params string[] columns) where T : class
        {
            string tableColumns = GetTableColumns<T>(columns);
            var name = GetTableName(typeof(T));
            using var conn = ConnectionManager.GetSqlConnection();
            return conn.QueryFirstOrDefault<T>(DefaultQuery.Top1(tableColumns, name));
        }

        public async Task<T> ReadAsync<T>(params string[] columns) where T : class
        {
            var tableColumns = GetTableColumns<T>(columns);
            var tableName = GetTableName(typeof(T));
            using var conn = ConnectionManager.GetSqlConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(DefaultQuery.Top1(tableColumns, tableName))
                .ConfigureAwait(false);
        }

        public IEnumerable<T> GetCollection<T>(params string[] columns) where T : class
        {
            if(columns.Any())
            {
                var tableColumns = ResolveColumns<T>(columns);
                var tableName = GetTableName(typeof(T));
                var query = DefaultQuery.Select(tableColumns, tableName);
                using var conn = ConnectionManager.GetSqlConnection();
                return conn.Query<T>(query);
            }
            else
            {
                using var conn = ConnectionManager.GetSqlConnection();
                return conn.GetAll<T>();
            }
        }

        public async Task<IEnumerable<T>> GetCollectionAsync<T>(params string[] columns) where T : class
        {
            if (columns.Any())
            {
                var tableColumns = ResolveColumns<T>(columns);
                var tableName = GetTableName(typeof(T));
                var query = DefaultQuery.Select(tableColumns, tableName);
                using var conn = ConnectionManager.GetSqlConnection();
                return await conn.QueryAsync<T>(query)
                    .ConfigureAwait(false);
            }
            else
            {
                using var conn = ConnectionManager.GetSqlConnection();
                return await conn.GetAllAsync<T>()
                    .ConfigureAwait(false);
            }
        }

        public IEnumerable<T> GetCollectionWhere<T>(string columnName, object value,
            params string[] columns) where T : class 
        {
            var tableColumns = GetTableColumns<T>(columns);
            var query = DefaultQuery.GetBy(tableColumns, GetTableName(typeof(T)), columnName);
            using var conn = ConnectionManager.GetSqlConnection();
            return conn.Query<T>(query, new { value });
        }

        public async Task<IEnumerable<T>> GetCollectionWhereAsync<T>(string columnName, object value,
            params string[] columns) where T : class
        {
            var tableColumns = GetTableColumns<T>(columns);
            var query = DefaultQuery.GetBy(tableColumns, GetTableName(typeof(T)), columnName);
            using var conn = ConnectionManager.GetSqlConnection();
            return await conn.QueryAsync<T>(query, new { value });
        }

        public bool Update<T>(T entity) where T : class
        {
            using var conn = ConnectionManager.GetSqlConnection();
            return conn.Update(entity);
        }

        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            using var conn = ConnectionManager.GetSqlConnection();
            return await conn.UpdateAsync(entity)
                .ConfigureAwait(false);
        }

        public void Delete<T>(T entity) where T : class
        {
            using var conn = ConnectionManager.GetSqlConnection();
            conn.Delete(entity);
        }

        private string GetTableName(Type t)
        {
            var items = _tableDefinitions.Where(i => i.DefType == t.Name);

            if (items.Any())
            {
                return items.First().TableName;
            }

            return ModelTracker.GetTableName(t);
        }

        private static string ResolveColumns<T>(string[] str)
        {
            if (str.Length == 0)
            {
                return string.Empty;
            }
            
            // Include entity id when user specifies columns
            // If it has one of course 
            var keyId = typeof(T).GetProperties().
                Where(i => i.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));

            // All but the last one
            var defaultColumns = string.Join(",", str);
            return $"{(keyId.Any() ? $"{keyId.First().Name}," : "")}{defaultColumns}";
        }

        private static PropertyInfo GetKeyId<T>() where T : class
        {
            return typeof(T).GetProperties().
            First(i => i.CustomAttributes.Any(a => a.AttributeType ==
            typeof(KeyAttribute)));
        }
        private static string GetTableColumns<T>(string[] columns) where T : class
        {
            // Potential bug, column query is searching for wont be included in final result
            return columns.Any() ? ResolveColumns<T>(columns) : "*";
        }
    }
}
