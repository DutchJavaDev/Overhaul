using System.Data.SqlClient;
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
        private const string SchemaCheckQuery = $"SELECT COUNT(*) " +
                          $"FROM INFORMATION_SCHEMA.TABLES " +
                          $"WHERE TABLE_NAME = @tableName;";

        public SqlConnection Connection { get; set; }

        public IEnumerable<TableDefinition> GetCollection()
        {
            // Wont work until insert is done, its done?
            using (Connection = ConnectionManager.GetSqlConnection())
            {
                return Connection.Query<TableDefinition>($"SELECT * FROM {ModelTracker.GetTableName(typeof(TableDefinition))}" +
                $" WHERE {nameof(TableDefinition.Id)} > 1");
            }
        }

        public bool CreateTable(TableDefinition tableDef)
        {
            using (Connection = ConnectionManager.GetSqlConnection())
            {
                var query = DefaultQuery.CreateTable(tableDef.TableName, tableDef.ColumnCollection);
                Connection.ExecuteScalar(query);
                Connection.Insert(tableDef);
            }

            return TableExists(tableDef.TableName);
        }

        public bool DeleteTable(string tableName)
        {
            if (!TableExists(tableName))
                return true;

            using (Connection = ConnectionManager.GetSqlConnection())
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
            // Only checks for tables within this database [sandbox i guess], im dumb
            using (Connection = ConnectionManager.GetSqlConnection())
            {
                if (int.TryParse(Connection
                    .ExecuteScalar(SchemaCheckQuery, new { tableName }).ToString(),
                    out var num))
                {
                    return num == 1;
                }
            }
            return false;
        }
    }
}
