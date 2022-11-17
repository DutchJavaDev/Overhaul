using System.Data;
using Overhaul.Data;
using System.Data.SqlClient;

namespace Overhaul.Interface
{
    internal interface ISqlGenerator
    {
        SqlConnectionStringBuilder ConnectionBuilder { get; init; }

        SqlConnection Connection { get; set; }
        
        string ConnectionString { get; init; }

        /// <summary>
        /// Gets a collection of all the defined 
        /// Tablefinitons
        /// </summary>
        /// <returns>Collection of TableDefinitions</returns>
        IEnumerable<TableDefinition> GetCollection();

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <param name="tableDef"></param>
        /// <returns>true If created</returns>
        bool CreateTable(TableDefinition tableDef);

        /// <summary>
        /// Deletes a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>true</returns>
        bool DeleteTable(string tableName);

        /// <summary>
        /// Checks if a table exists with the given name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if exists</returns>
        bool TableExists(string name);
    }
}
