using System.Data.SqlClient;

namespace Overhaul.Interface
{
    internal interface ISqlModifier
    {
        SqlConnectionStringBuilder ConnectionBuilder { get; init; }

        SqlConnection Connection { get; set; }

        string ConnectionString { get; init; }
        /// <summary>
        /// Adds a column to an existing table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="nColumn"></param>
        /// <returns>true if succeded</returns>
        bool AddColumn(string tableName, string nColumn);

        /// <summary>
        /// Deletes or marks column as nullable
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns>true if succeded</returns>
        bool DeleteColumn(string tableName, string column);
    }
}
