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
        IEnumerable<TableDef> GetCollection();

        bool CreateTable(TableDef tableDef);

        bool DeleteTable(TableDef tableDef);

        bool TableExists(string name);
    }
}
