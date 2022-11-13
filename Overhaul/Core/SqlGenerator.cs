using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Dapper;
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

        public SqlGenerator(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionBuilder = new (connectionString);
        }

        public IEnumerable<TableDefinition> GetCollection()
        {
            // Wont work until insert is done 
            using (Connection = Create())
            { 
                var sql = $"SELECT * FROM {ModelTracker.GetTableName(typeof(TableDefinition))}";
        
                return Connection.Query<TableDefinition>(sql);
            }
        }

        public bool CreateTable(TableDefinition tableDef)
        {
            using (Connection = Create())
            {
                var sql = $"CREATE TABLE {tableDef.TableName} " +
                    $"({tableDef.ColumnCollection.Replace(";",",")})";

                Connection.ExecuteScalar(sql);
            }

            return TableExists(tableDef.TableName);
        }

        public bool DeleteTable(string tableName)
        {
            if (!TableExists(tableName))
                    return true;

            using (Connection = Create())
            {
                tableName = ModelTracker.GetTableName(tableName);

                var sql = $"DROP TABLE {tableName}";

                Connection.ExecuteScalar(sql);
            }

            return !TableExists(tableName);
        }

        public bool TableExists(string name)
        {
            // Only checks for tables within this databatse [sandbox i guess]
            using (Connection = Create())
            {
                name = ModelTracker.GetTableName(name);

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
