using System.Data.SqlClient;

namespace Overhaul.Interface
{
    internal interface ISqlModifier
    {
        SqlConnectionStringBuilder ConnectionBuilder { get; init; }

        SqlConnection Connection { get; set; }

        string ConnectionString { get; init; }
        bool AddColumn(string tableName, string nColumn);

        bool DeleteColumn(string columnName);
    }
}
