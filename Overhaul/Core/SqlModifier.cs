using Dapper;
using Overhaul.Interface;
using System.Data.Common;
using System.Data.SqlClient;

namespace Overhaul.Core
{
    public sealed class SqlModifier : ISqlModifier
    {
        public SqlConnectionStringBuilder ConnectionBuilder { get; init; }
        public SqlConnection Connection { get; set; }

        public string ConnectionString { get; init; }

        public SqlModifier(string connectionString) 
        {
            ConnectionString = connectionString;
            ConnectionBuilder = new(connectionString);
        }

        public bool AddColumn(string tableName, string nColumn)
        {
            var sql = $"ALTER TABLE {tableName} ADD {nColumn}";

            using (var con = Create())
            {
                con.ExecuteScalar(sql);
            }

            return true;
        }
        public bool UpdateColumn(string tableName, string nColumn)
        {
            var sql = $"ALTER TABLE {tableName} ALTER COLUMN {nColumn}";

            using (var con = Create())
            {
                con.ExecuteScalar(sql);
            }
            return true;
        }

        public bool DeleteColumn(string tableName, string column)
        {
            // Data loss protection for now, see what happens 
            var sql = $"ALTER TABLE {tableName} ALTER COLUMN {column}" +
                $"{(column.Contains("NULL") ? "" : " NULL")}";

            using (var con = Create())
            {
                con.ExecuteScalar(sql);
            }
            return true;
        }

        private SqlConnection Create()
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            return Connection;
        }

    }
}
