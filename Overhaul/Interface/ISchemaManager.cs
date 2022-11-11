using Overhaul.Data;

namespace Overhaul.Interface
{
    internal interface ISchemaManager
    {
        void RunSchemaCreate(IEnumerable<TableDefinition> addedTables);
        void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables, 
            IEnumerable<TableDefinition> _cache);
        void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables);
    }
}
