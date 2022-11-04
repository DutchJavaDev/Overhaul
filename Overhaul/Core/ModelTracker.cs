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
                RunSchemaCreate(definitions.Where(i => 
                !_cache.Any(ii => ii.DefType == i.DefType)));

                // DefType are the same but columns don't match up
                RunSchemaUpdate(definitions.Where(i
                    => _cache.Any(ii => ii.DefType == i.DefType && !i.Equals(ii))));
            }
        }

        internal static void RunSchemaCreate(IEnumerable<TableDefinition> addedTables)
        {
            foreach(var table in addedTables)
            {
                sqlGenerator.CreateTable(table);
            }
        }

        internal static void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTabes)
        {
            foreach (var table in modifiedTabes)
            {
                var oldType = _cache.Where(i => i.DefType == table.DefType).First();

                var oldTypeColumnArray = Supported.ConvertTypesStringToArray(oldType.ColumnCollection);

                var newTypeColumnArray = Supported.ConvertTypesStringToArray(table.ColumnCollection);

                var addedColumns = Supported.GetAddedColumns(oldTypeColumnArray, newTypeColumnArray);

                var deletedColumns = Supported.GetDeletedColumns(oldTypeColumnArray, newTypeColumnArray);

                foreach(var column in addedColumns)
                {
                    sqlModifier.AddColumn(table.TableName, column);
                }

                foreach(var column in deletedColumns)
                {
                    sqlModifier.DeleteColumn(table.TableName, column);
                }
            }
        }


        internal static IEnumerable<TableDefinition> LoadCache()
        {
            var def = BuildDef(typeof(TableDefinition));

            if (!sqlGenerator.TableExists(def.TableName))
            {
                sqlGenerator.CreateTable(def);


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
            return types.Select(i => BuildDef(i));
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
