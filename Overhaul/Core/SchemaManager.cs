using System.Runtime.CompilerServices;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class SchemaManager : ISchemaManager
    {
        private readonly ISqlGenerator sqlGenerator;
        private readonly ISqlModifier sqlModifier;
        public SchemaManager(ISqlGenerator gen, ISqlModifier mod)
        {
            sqlGenerator = gen;
            sqlModifier = mod;
        }

        public void RunSchemaCreate(IEnumerable<TableDefinition> addedTables)
        {
            addedTables.AsParallel().ForAll(i => sqlGenerator.CreateTable(i));
        }

        public void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables, 
            IEnumerable<TableDefinition> _cache)
        {
            modifiedTables.AsParallel().ForAll(newType =>
            {
                var oldType = _cache.Where(i => i.DefType == newType.DefType
#if DEBUG
                || i.TableName == newType.TableName
#endif
                ).FirstOrDefault();
                GetChanges(newType, oldType, out IEnumerable<string> addedColumns, out IEnumerable<string> deletedColumns);
                deletedColumns.AsParallel().ForAll(column => sqlModifier.DeleteColumn(newType.TableName, column));
                addedColumns.AsParallel().ForAll(column => sqlModifier.AddColumn(newType.TableName, column));
            });
        }
        public void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables)
        {
            deleteTables.AsParallel().ForAll(i => sqlGenerator.DeleteTable(i.TableName));
        }

        private static void GetChanges(TableDefinition table, TableDefinition oldType,
            out IEnumerable<string> addedColumns, out IEnumerable<string> deletedColumns)
        {
            var oldTypeColumnArray = Supported.ConvertTypesStringToArray(oldType.ColumnCollection);
            var newTypeColumnArray = Supported.ConvertTypesStringToArray(table.ColumnCollection);
            addedColumns = Supported.GetAddedColumns(oldTypeColumnArray, newTypeColumnArray);
            deletedColumns = Supported.GetDeletedColumns(oldTypeColumnArray, newTypeColumnArray);
        }
    }
}
