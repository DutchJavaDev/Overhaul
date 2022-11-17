using Overhaul.Data;

namespace Overhaul.Interface
{
    internal interface ISchemaManager
    {
        /// <summary>
        /// Creates tables that don't exists yet
        /// </summary>
        /// <param name="addedTables"></param>
        void RunSchemaCreate(IEnumerable<TableDefinition> addedTables);

        /// <summary>
        /// Updates existins tables columns
        /// </summary>
        /// <param name="modifiedTables"></param>
        /// <param name="_cache"></param>
        void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables, 
            IEnumerable<TableDefinition> _cache);

        /// <summary>
        /// Deletes existing tables
        /// </summary>
        /// <param name="deleteTables"></param>
        void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables);
    }
}
