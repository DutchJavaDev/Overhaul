using System.Data.SqlClient;
using System.Runtime.CompilerServices;

using Dapper;
using Dapper.Contrib.Extensions;

using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class Crud : ICrud
    {
        private readonly IEnumerable<TableDefinition> _tableDefCache;
        private readonly string ConnectionString;

        public Crud(IEnumerable<TableDefinition> tables, 
            string connectionString)
        {
            _tableDefCache = tables;
            ConnectionString = connectionString;
        }
        public T Create<T>(T entity) where T : class
        {
            using var conn = Create();
            conn.Insert(entity);
            return entity;
        }
        public async Task<T> CreateAsync<T>(T entity) where T : class
        {
            return await Task.Run(() => Create(entity))
                .ConfigureAwait(false);
        }
        public T GetById<T>(object id, params string[] columns) 
            where T : class
        {
            SqlConnection conn;

            // Only works if entity has key attribute
            if (columns.Any())
            {
                var keyId = typeof(T).GetProperties().
                First(i => i.CustomAttributes.Any(a => a.AttributeType == 
                typeof(KeyAttribute)));

                if(keyId == null)
                {
                    throw new Exception($"{nameof(keyId)} is null");
                }

                var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";

                var sql = $"SELECT {columnSql},{keyId.Name} FROM " +
                          $"{GetTableName(typeof(T))} " +
                          $"WHERE {keyId.Name} = @id";
                var parameters = new 
                {
                    id
                };

                using (conn = Create()) 
                {    
                   return conn.QuerySingle<T>(sql,parameters);
                }
            }

            using (conn = Create())
            {
                return conn.Get<T>(id);
            }
        }
        public async Task<T> GetByIdAsync<T>(object id, params string[] columns)
            where T : class
        {
            SqlConnection conn;

            // Only works if entity has key attribute
            if (columns.Any())
            {
                var keyId = typeof(T).GetProperties().
                First(i => i.CustomAttributes.Any(a => a.AttributeType ==
                typeof(KeyAttribute)));

                if (keyId == null)
                {
                    throw new Exception($"{nameof(keyId)} is null");
                }

                var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";

                var sql = $"SELECT {columnSql},{keyId.Name} FROM " +
                          $"{GetTableName(typeof(T))} " +
                          $"WHERE {keyId.Name} = @id";
                var parameters = new
                {
                    id
                };

                using (conn = await CreateAsync())
                {
                    return await conn.QuerySingleAsync<T>(sql, parameters)
                        .ConfigureAwait(false);
                }
            }

            using (conn = await CreateAsync())
            {
                return await conn.GetAsync<T>(id)
                    .ConfigureAwait(false);
            }
        }
        public T GetBy<T>(string columnName, object value, params string[] columns) 
            where T : class
        {
            // Potential bug, column query is searching for wont be included in final result
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            string sql = $"SELECT TOP 1 {columnSql} FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = @value";
            var parameters = new 
            {
                value
            };
            using var conn = Create();
            return conn.QuerySingle<T>(sql,parameters);
        }
        public async Task<T> GetByAsync<T>(string columnName, object value, params string[] columns)
            where T : class
        {
            // Potential bug, column query is searching for wont be included in final result
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            string sql = $"SELECT TOP 1 {columnSql} FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = @value";
            var parameters = new
            {
                value
            };
            using var conn = await CreateAsync();
            return await conn.QuerySingleAsync<T>(sql, parameters)
                .ConfigureAwait(false);
        }
        public T Read<T>(params string[] columns) where T : class
        {
            // Potential bug, column query is searching for wont be included in final result
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            var name = GetTableName(typeof(T));
            using var conn = Create();
            return conn.QueryFirstOrDefault<T>($"SELECT TOP 1 {columnSql} FROM {name}");
        }
        public async Task<T> ReadAsync<T>(params string[] columns) where T : class
        {
            // Potential bug, column query is searching for wont be included in final result
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            var name = GetTableName(typeof(T));
            using var conn = Create();
            return await conn.QueryFirstOrDefaultAsync<T>($"SELECT TOP 1 {columnSql} FROM {name}")
                .ConfigureAwait(false);
        }
        public IEnumerable<T> GetCollection<T>(params string[] columns) where T : class
        {
            if(columns.Any())
            {
                var columnSql = ResolveColumns<T>(columns);
                var name = GetTableName(typeof(T));
                var sql = $"SELECT {columnSql} FROM {name}";
                using var conn = Create();
                return conn.Query<T>(sql);
            }
            else
            {
                using var conn = Create();
                return conn.GetAll<T>();
            }
        }
        public async Task<IEnumerable<T>> GetCollectionAsync<T>(params string[] columns) where T : class
        {
            if (columns.Any())
            {
                var columnSql = ResolveColumns<T>(columns);
                var name = GetTableName(typeof(T));
                var sql = $"SELECT {columnSql} FROM {name}";
                using var conn = Create();
                return await conn.QueryAsync<T>(sql)
                    .ConfigureAwait(false);
            }
            else
            {
                using var conn = Create();
                return await conn.GetAllAsync<T>()
                    .ConfigureAwait(false);
            }
        }
        public IEnumerable<T> GetCollectionWhere<T>(string columnName, object value,
            params string[] columns) where T : class 
        {
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            string sql = $"SELECT {columnSql} FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = @value";
            var parameters = new 
            {
                value
            };
            using var conn = Create();
            return conn.Query<T>(sql,parameters);
        }
        public async Task<IEnumerable<T>> GetCollectionWhereAsync<T>(string columnName, object value,
            params string[] columns) where T : class
        {
            var columnSql = columns.Any() ? ResolveColumns<T>(columns) : "*";
            string sql = $"SELECT {columnSql} FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = @value";
            var parameters = new
            {
                value
            };
            using var conn = Create();
            return await conn.QueryAsync<T>(sql, parameters);
        }
        public bool Update<T>(T entity) where T : class
        {
            using var conn = Create();
            return conn.Update(entity);
        }
        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            using var conn = Create();
            return await conn.UpdateAsync(entity)
                .ConfigureAwait(false);
        }
        public void Delete<T>(T entity) where T : class
        {
            using var conn = Create();
            conn.Delete(entity);
        }

        private SqlConnection Create()
        {
            var sql = new SqlConnection(ConnectionString);
            sql.Open();
            return sql;
        }

        private async Task<SqlConnection> CreateAsync()
        {
            var sql = new SqlConnection(ConnectionString);
            await sql.OpenAsync();
            return sql;
        }

        private string GetTableName(Type t)
        {
            var items = _tableDefCache.Where(i => i.DefType == t.Name);

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
    }
}
