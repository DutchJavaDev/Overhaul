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
        IEnumerable<TableDefinition> GetCollection();

        bool CreateTable(TableDefinition tableDef);

        bool DeleteTable(string tableName);

        bool TableExists(string name);
    }
}
