using Dapper;
using Overhaul.Data;
using Overhaul.Interface;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class SqlGenerator : ISqlGenerator
    {
        public SqlConnectionStringBuilder ConnectionBuilder { get; init; }
        public SqlConnection Connection { get; set; }
        public string ConnectionString { get; init; }

        public SqlGenerator(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionBuilder = new (connectionString);
        }

        public bool CreateTable(TableDef tableDef)
        {
            using (Connection = Create())
            {
                var sql = $"CREATE TABLE {tableDef.TableName} ({tableDef.ColumnCollection})";

                Connection.ExecuteScalar(sql);
            }

            return TableExists(tableDef.TableName);
        }

        public bool DeleteTable(TableDef tableDef)
        {
            using (Connection = Create())
            {
                var sql = $"DROP TABLE {tableDef.TableName}";

                Connection.ExecuteScalar(sql);
            }

            return !TableExists(tableDef.TableName);
        }

        public bool TableExists(string name)
        {
            // Only checks for tables within this databatse [sandbox i guess]
            using (Connection = Create())
            {
                var sql = $"SELECT COUNT(*) " +
                          $"FROM INFORMATION_SCHEMA.TABLES " +
                          $"WHERE TABLE_NAME = @tableName;";

                var parameters = new { tableName = name };

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
