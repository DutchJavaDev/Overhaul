using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class SqlGenerator : ISqlGenerator
    {
        public SqlConnectionStringBuilder ConnectionBuilder { get; init; }
        public SqlConnection Connection { get; set; }
        public string ConnectionString { get; init; }
        private readonly string GetCollectionQuery;

        public SqlGenerator(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionBuilder = new (connectionString);
            GetCollectionQuery = $"SELECT * FROM {ModelTracker.GetTableName(typeof(TableDefinition))}" +
                $" WHERE {nameof(TableDefinition.Id)} > 1";
        }

        public IEnumerable<TableDefinition> GetCollection()
        {
            // Wont work until insert is done 
            using (Connection = Create())
            { 
                return Connection.Query<TableDefinition>(GetCollectionQuery);
            }
        }

        public bool CreateTable(TableDefinition tableDef)
        {
            using (Connection = Create())
            {
                var query = DefaultQuery.CreateTable(tableDef.TableName,tableDef.ColumnCollection);
                Connection.ExecuteScalar(query);
                Connection.Insert(tableDef);
            }

            return TableExists(tableDef.TableName);
        }

        public bool DeleteTable(string tableName)
        {
            if (!TableExists(tableName))
                    return true;

            using (Connection = Create())
            {
                var sql = $"DROP TABLE {tableName}";
                var sqlTableDef = $"DELETE FROM {ModelTracker.GetTableName(typeof(TableDefinition))} WHERE TableName = @tableName";

                Connection.ExecuteScalar(sqlTableDef, new { tableName });
                Connection.ExecuteScalar(sql);
            }

            return !TableExists(tableName);
        }

        public bool TableExists(string tableName)
        {
            // Only checks for tables within this database [sandbox i guess]
            using (Connection = Create())
            {
                var sql = $"SELECT COUNT(*) " +
                          $"FROM INFORMATION_SCHEMA.TABLES " +
                          $"WHERE TABLE_NAME = @tableName;";

                var parameters = new { tableName = tableName };

                if (int.TryParse(Connection
                    .ExecuteScalar(sql, parameters).ToString(), 
                    out var num))
                {
                    return num == 1;
                }
            }
            return false;
        }

        private SqlConnection Create()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
