using Overhaul.Interface;
using System.Data.SqlClient;

namespace Overhaul.Core
{
    internal sealed class SqlModifier : ISqlModifier
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
            return true;
        }

        public bool DeleteColumn(string columnName)
        {
            return false;
        }
    }
}
