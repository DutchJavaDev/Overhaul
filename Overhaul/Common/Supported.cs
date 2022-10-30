using Dapper.Contrib.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Common
{
    internal static class Supported
    {
        private readonly static Dictionary<Type, string> SqlTypes = new()
        {
            {typeof(int), "INT"},
            {typeof(double), "FLOAT"},
            {typeof(string), "NVARCHAR(255)"},
            {typeof(bool), "BIT" },
            {typeof(Guid), "UNIQUEIDENTIFIER"},
            {typeof(float), "FLOAT" },
            {typeof(byte), "TINYINT" },
            {typeof(short), "SMALLINT" },
            {typeof(decimal), "DECIMAL" },
            {typeof(DateTime), "DATETIME"},
            {typeof(char), "CHAR(2)" }
        };

        private readonly static List<Type> SqlIdentityTypes = new()
        {
            typeof(int),
            typeof(Guid),
        };

        public static PropertyInfo[] GetPropertiesForType(Type type)
        {
            return type.GetProperties().Where(i => ValidProperty(i))
                .ToArray();
        }

        public static string ConvertPropertiesToTypesString(PropertyInfo[] types, out int count)
        {
            var validColumns = types.Where(i => SqlTypes.ContainsKey(i.PropertyType))
                .Select(i => GetColumnDefinition(i));

            var builder = new StringBuilder();

            count = 0;

            foreach(var column in validColumns)
            {
                builder.Append($"{column}{(column == validColumns.Last() ? "" : ",")}");
                count++;
            }

            return builder.ToString();
        }

        private static string GetColumnDefinition(PropertyInfo property)
        {
            var name = property.Name;
            var type = property.PropertyType;
            var column = SqlTypes[type];

            if (IsIdentityType(type))
            {
                column += " IDENTITY(1,1) ";
            }

            return $"{name} {column}";
        }

        private static bool IsIdentityType(Type type)
        {
            return Attribute.IsDefined(type, typeof(KeyAttribute))
                && SqlIdentityTypes.Contains(type);
        }

        internal static bool ValidProperty(PropertyInfo info)
        {
            var read = info.CanRead;
            var write = info.CanWrite;
            // var primitieve = info.DeclaringType.IsPrimitive; does not support datetime && guid
            var publicSet = info.GetSetMethod(false)?.IsPublic;
            var publicGet = info.GetGetMethod(false)?.IsPublic;

            return read && write &&
                publicSet.Value &&
                publicGet.Value;
        }
    } 
}
