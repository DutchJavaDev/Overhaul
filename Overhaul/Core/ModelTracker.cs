using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal static class ModelTracker
    {
        private static IEnumerable<TableDefinition> _cache;
        private static ISqlGenerator sqlGenerator;
        private static ISqlModifier sqlModifier;
        private static ISchemaManager schemaManager;
        private static ICrud crud;

        public static void Track(IEnumerable<Type> types, string connectionString = "")
        {
            // If debug read connectionstring from secrets?
            sqlGenerator = new SqlGenerator(connectionString);
            sqlModifier = new SqlModifier(connectionString);

            // Check Db
            _cache = LoadCache();

            schemaManager = new SchemaManager(sqlGenerator,sqlModifier);
            
            // Create definitions
            var definitions = CreateDefinitions(types);
            
            if (definitions.Any() && _cache.Any())
            {
                schemaManager.RunSchemaDelete(_cache.Where(i => !definitions.Any(ii => i == ii)));

                schemaManager.RunSchemaCreate(definitions.Where(i => 
                !_cache.Any(ii => ii.DefType == i.DefType)));

                // DefType are the same but columns don't match up
                // ovvriden queals for tableDef
                schemaManager.RunSchemaUpdate(definitions.Where(i
                    => _cache.Any(ii => ii.DefType == i.DefType && !i.Equals(ii))), _cache);
            }

            crud = new Crud(_cache, connectionString);
        }

        internal static ICrud GetCrudInstance()
        {
            return crud;
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
                return table.Name;
            }
            return type.Name;
        }

        public static void DeleteTestTables(Type[] tables, string conn = "")
        {

            if (sqlGenerator == null || !string.IsNullOrEmpty(conn))
            {
                sqlGenerator = new SqlGenerator(conn);
            }

            var types = tables.Select(i => BuildDef(i));

            foreach (var table in types)
            {
                sqlGenerator.DeleteTable(table.TableName);
            }
        }
    }
}
