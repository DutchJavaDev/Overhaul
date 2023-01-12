using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    public sealed class ModelTracker : IModelTracker
    {
        internal static ModelTrackerOptions Options = new();
        private readonly string ConnectionString;
        private readonly ISqlGenerator sqlGenerator;
        private readonly ISchemaManager schemaManager;
        private readonly IEnumerable<TableDefinition> databaseDefinitions;

        public ModelTracker(string connectionString, ModelTrackerOptions options = null)
        {
            Options = options ?? new ModelTrackerOptions();

            sqlGenerator = new SqlGenerator(connectionString);

            databaseDefinitions = LoadDatabaseDefinitons();
            
            schemaManager = new SchemaManager(connectionString);

            ConnectionString = connectionString;
        }

        public void Track(IEnumerable<Type> types)
        {
            var definitions = CreateDefinitions(types);
            
            if (definitions.Any() || databaseDefinitions.Any())
            {
                CheckForAddedDefinitions(definitions);
                CheckForUpdatedDefinitions(definitions);
                CheckForDeletedDefinitions(definitions);
            }
        }
        private void CheckForDeletedDefinitions(IEnumerable<TableDefinition> definitions)
        {
            var deletedDefinitions = databaseDefinitions.Where(i => !definitions.Any(ii => ii.TableName == i.TableName));

            if (deletedDefinitions.Any())
            {
                schemaManager.RunSchemaDelete(databaseDefinitions.Where(i => !definitions.Any(ii => i.Equals(ii))));
            }
        }
        private void CheckForUpdatedDefinitions(IEnumerable<TableDefinition> definitions)
        {
            var updatedDefinitions = definitions.Where(i
                                => databaseDefinitions.Any(ii => ii.DefType == i.DefType && !i.Equals(ii)));

            if (updatedDefinitions.Any())
            {
                // DefType are the same but columns don't match up
                // overridden equals for tableDef
                schemaManager.RunSchemaUpdate(updatedDefinitions, databaseDefinitions);

            }
        }
        private void CheckForAddedDefinitions(IEnumerable<TableDefinition> definitions)
        {
            var addedDefinitons = definitions.Where(i =>
                            !databaseDefinitions.Any(ii => ii.TableName == i.TableName));

            if (addedDefinitons.Any())
            {
                schemaManager.RunSchemaCreate(addedDefinitons);
            }
        }
        public ICrud GetCrudInstance()
        {
            return new Crud(databaseDefinitions, ConnectionString);
        }

        private IEnumerable<TableDefinition> LoadDatabaseDefinitons()
        {
            var def = BuildDefinitions(typeof(TableDefinition));

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
            return types.Select(i => BuildDefinitions(i));
        }

        // Move to class
        private static TableDefinition BuildDefinitions(Type type)
        {
            var tableName = GetTableName(type);
            var properties = Supported.GetTypeProperties(type);
            var columnCollection = 
                Supported.ConvertPropertiesToTypesString(properties, out var count);

            return new() 
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
#if DEBUG 
        // Only needed when debugging, running test
        public static void DeleteTestTables(Type[] tables, string conn = "", bool deleteDef = false)
        {
            var sqlGenerator = new SqlGenerator(conn);
            var del = tables.ToList();

            if (deleteDef)
            {
                // Auto delete this fucker
                del.Add(typeof(TableDefinition));
            }

            var types = del.Select(i => BuildDefinitions(i)).ToList();

            foreach (var table in types)
            {
                if (sqlGenerator.TableExists(table.TableName))
                {
                    sqlGenerator.DeleteTable(table.TableName);
                }
            }
        }
#endif
    }
}
