using System.Data.SqlClient;

namespace Overhaul.Core
{
    internal static class ConnectionManager
    {
        private static string ConnectionString { get; set; }
        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public static SqlConnection GetSqlConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public static async Task<SqlConnection> GetSqlConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
