using System.Reflection;
using System.Runtime.CompilerServices;
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
        private string ConnectionString { get; init; }

        private static IEnumerable<TableDefinition> _cache;
        private readonly ISqlGenerator sqlGenerator;
        private readonly ISqlModifier sqlModifier;
        private readonly ISchemaManager schemaManager;

        public ModelTracker(string connectionString, ModelTrackerOptions options = null)
        {
            options ??= new ModelTrackerOptions();

            Options = options;

            sqlGenerator = new SqlGenerator(connectionString);
            sqlModifier = new SqlModifier(connectionString);

            // Check Db
            _cache = LoadDatabaseCace();
            
            schemaManager = new SchemaManager(sqlGenerator,sqlModifier);

            ConnectionString = connectionString;
        }

        public void Track(IEnumerable<Type> types)
        {
            // Create definitions
            var definitions = CreateDefinitions(types);
            
            if (definitions.Any() || _cache.Any())
            {
                schemaManager.RunSchemaCreate(definitions.Where(i => 
                !_cache.Any(ii => ii.DefType == i.DefType)));

                // DefType are the same but columns don't match up
                // overridden equals for tableDef
                schemaManager.RunSchemaUpdate(definitions.Where(i
                    => _cache.Any(ii => ii.DefType == i.DefType && !i.Equals(ii))), _cache);
                
                schemaManager.RunSchemaDelete(_cache.Where(i => !definitions.Any(ii => i.Equals(ii))));
            }
        }

        public ICrud GetCrudInstance()
        {
            return new Crud(_cache, ConnectionString);
        }

        private IEnumerable<TableDefinition> LoadDatabaseCace()
        {
            var def = BuildDef(typeof(TableDefinition));

            if (!sqlGenerator.TableExists(def.TableName))
            {
                sqlGenerator.CreateTable(def);
                // Don't return def since it has just been created
                // Otherwise it will trigger a create table ... again
                return Enumerable.Empty<TableDefinition>();
            }

            return sqlGenerator.GetCollection()
                .Where(i => i.Id > 1); // Skipping the TableDefinition for TableDefinition,
                                       // otherwise it will get deleted since its not part of the 'new' types
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

            var types = del.Select(i => BuildDef(i)).ToList();

            foreach (var table in types)
            {
                sqlGenerator.DeleteTable(table.TableName);
            }
        }
#endif
    }
}
