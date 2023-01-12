using Overhaul.Data;
using System.Data.SqlClient;

namespace Overhaul.Interface
{
    internal interface ISqlGenerator
    {
        SqlConnection Connection { get; set; }
        
        IEnumerable<TableDefinition> GetCollection();

        bool CreateTable(TableDefinition tableDef);

        bool DeleteTable(string tableName);

        bool TableExists(string name);
    }
}
