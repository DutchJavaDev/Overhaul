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
        public T GetById<T>(object id) where T : class
        {
            using var conn = Create();
            return conn.Get<T>(id);
        }
        public T GetBy<T>(string columnName, object value) where T : class
        {
            string sql = $"SELECT TOP 1 * FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = '{value}'";
            using var conn = Create();
            return conn.QuerySingle<T>(sql);
        }
        public T Read<T>() where T : class
        {
            var name = GetTableName(typeof(T));
            using var conn = Create();
            return conn.QueryFirstOrDefault<T>($"SELECT TOP 1 * FROM {name}");
        }
        public IEnumerable<T> GetCollection<T>() where T : class
        {
            using var conn = Create();
            return conn.GetAll<T>();
        }
        public IEnumerable<T> GetCollectionWhere<T>(string columnName, object value) where T : class 
        {
            string sql = $"SELECT * FROM {GetTableName(typeof(T))} " +
                $"WHERE {columnName} = '{value}'";
            using var conn = Create();
            return conn.Query<T>(sql);
        }
        public bool Update<T>(T entity) where T : class
        {
            using var conn = Create();
            return conn.Update(entity);
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
        private string GetTableName(Type t)
        {
            var items = _tableDefCache.Where(i => i.DefType == t.Name);

            if (items.Any())
            {
                return items.First().TableName;
            }

            return ModelTracker.GetTableName(t);
        }
    }
}
