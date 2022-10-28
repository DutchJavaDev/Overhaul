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
            // Create defenitions
            _cache = CreateDefenitions(types);

            // 

            // If debug read connectionstring from secrets
        }

        internal static IEnumerable<TableDef> CreateDefenitions(IEnumerable<Type> types)
        {
            return types.Select(i => BuildDef(i));
        }

        private static TableDef BuildDef(Type type)
        {
            var tableName = GetTableName(type);
            var properties = Supported.GetPropertiesForType(type);
            var columnCollection = Supported.ConvertPropertiesToTypesString(properties);

            return new TableDef 
            {
                TableName = tableName,
                ColumnCollection = columnCollection,
                DefType = type.Name
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
