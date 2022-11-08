using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal static class ModelTracker
    {
        private static readonly string _tablePrefix = "tbl";
        private static IEnumerable<TableDefinition> _cache;
        private static ISqlGenerator sqlGenerator;
        private static ISqlModifier sqlModifier;

        public static void Track(IEnumerable<Type> types, string connectionString = "")
        {
            // If debug read connectionstring from secrets?
            sqlGenerator = new SqlGenerator(connectionString);

            // Create definitions
            var definitions = CreateDefinitions(types);
            
            // Check Db
            _cache = LoadCache();

            if (definitions.Any())
            {
                RunSchemaDelete(_cache.Where(i => !definitions.Any(ii => i == ii)));

                RunSchemaCreate(definitions.Where(i => 
                !_cache.Any(ii => ii.DefType == i.DefType)));

                // DefType are the same but columns don't match up
                RunSchemaUpdate(definitions.Where(i
                    => _cache.Any(ii => ii.DefType == i.DefType && !i.Equals(ii))));
            }
        }

        internal static void RunSchemaCreate(IEnumerable<TableDefinition> addedTables)
        {
            addedTables.AsParallel().ForAll(i => sqlGenerator.CreateTable(i));
        }

        internal static void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables)
        {
            deleteTables.AsParallel().ForAll(i => sqlGenerator.DeleteTable(i.TableName));
        }

        internal static void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables)
        {
            modifiedTables.AsParallel().ForAll(newType =>
            {
                var oldType = _cache.Where(i => i.DefType == newType.DefType).First();
                GetChanges(newType, oldType, out IEnumerable<string> addedColumns, out IEnumerable<string> deletedColumns);
                addedColumns.AsParallel().ForAll(column => sqlModifier.AddColumn(newType.TableName, column));
                deletedColumns.AsParallel().ForAll(column => sqlModifier.DeleteColumn(newType.TableName, column));
            });
        }

        private static void GetChanges(TableDefinition table, TableDefinition oldType,
            out IEnumerable<string> addedColumns, out IEnumerable<string> deletedColumns)
        {
            var oldTypeColumnArray = Supported.ConvertTypesStringToArray(oldType.ColumnCollection);
            var newTypeColumnArray = Supported.ConvertTypesStringToArray(table.ColumnCollection);
            addedColumns = Supported.GetAddedColumns(oldTypeColumnArray, newTypeColumnArray);
            deletedColumns = Supported.GetDeletedColumns(oldTypeColumnArray, newTypeColumnArray);
        }

        internal static IEnumerable<TableDefinition> LoadCache()
        {
            var def = BuildDef(typeof(TableDefinition));

            if (!sqlGenerator.TableExists(def.TableName))
            {
                sqlGenerator.CreateTable(def);
                // Define system tables
                // Don't return def since it has just been created
                // Otherwise it will trigger a create table ... again
                return Enumerable.Empty<TableDefinition>();
            }

            return sqlGenerator.GetCollection();
        }

        // Move to class
        internal static IEnumerable<TableDefinition> 
            CreateDefinitions(IEnumerable<Type> types)
        {
            return types.AsParallel().Select(i => BuildDef(i));
        }

        // Move to class
        private static TableDefinition BuildDef(Type type)
        {
            var tableName = GetTableName(type);
            var properties = Supported.GetPropertiesForType(type);
            var columnCollection = 
                Supported.ConvertPropertiesToTypesString(properties, out var count);

            return new TableDefinition 
            {
                TableName = tableName,
                ColumnCollection = columnCollection,
                DefType = type.Name,
                ColumnCount = count,
            };
        }

        // Move to class
        internal static string GetTableName(Type type)
        {
            if (Attribute.IsDefined(type, typeof(TableAttribute))
                && type.GetCustomAttribute(typeof(TableAttribute)) 
                is TableAttribute table)
            {
                return $"{_tablePrefix}{table.Name}";
            }
            return $"{_tablePrefix}{type.Name}";
        }
    }
}
