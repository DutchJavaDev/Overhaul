using Dapper.Contrib.Extensions;
using Overhaul.Common;
using Overhaul.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal static class ModelTracker
    {
        private static readonly string _tablePrefix = "tbl";
        private static IEnumerable<TableDef> _cache;
        public static void Track(IEnumerable<Type> types, string connectionString = "")
        {
            // If debug read connectionstring from secrets
            
            // Check Db
            _cache = LoadCache();

            // Create definitions
            var definitions = CreateDefinitions(types);

        }

        internal static IEnumerable<TableDef> LoadCache()
        {
            return Enumerable.Empty<TableDef>();
        }

        internal static IEnumerable<TableDef> 
            CreateDefinitions(IEnumerable<Type> types)
        {
            return types.Select(i => BuildDef(i));
        }

        private static TableDef BuildDef(Type type)
        {
            var tableName = GetTableName(type);
            var properties = Supported.GetPropertiesForType(type);
            var columnCollection = 
                Supported.ConvertPropertiesToTypesString(properties, out var count);

            return new TableDef 
            {
                TableName = tableName,
                ColumnCollection = columnCollection,
                DefType = type.Name,
                ColumnCount = count
            };
        }

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
