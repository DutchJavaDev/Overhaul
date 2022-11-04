using Dapper.Contrib.Extensions;

using Overhaul.Data.Attributes;

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

        public static string[] ConvertTypesStringToArray(string types)
        {
            // Redo this lol
            var strA = new List<string>();
            foreach (var type in types.Split(","))
            {
                strA.Add(type.Trim());
            }
            return strA.ToArray();
        }

        public static IEnumerable<string> GetAddedColumns(string[] self, string[] other)
        {
            return GetDifferenceFromCollections(other,self);
        }

        public static IEnumerable<string> GetDeletedColumns(string[] self, string[] other)
        {
            return GetDifferenceFromCollections(self, other);
        }

        private static IEnumerable<T> GetDifferenceFromCollections<T>(IEnumerable<T> source, IEnumerable<T> target)
        {
            return source.Where(i => !target.Contains(i));
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

            if (IsDefined(property, typeof(StringPrecisionAttribute))
                && property.GetCustomAttribute<StringPrecisionAttribute>() is
                StringPrecisionAttribute strab)
            {
                column = column.Replace("(255)",strab.Precision);
            }

            return $"{name} {column}";
        }

        private static bool IsIdentityType(Type type)
        {
            return IsDefined(type, typeof(KeyAttribute))
                && SqlIdentityTypes.Contains(type);
        }
        private static bool IsDefined(MemberInfo info, Type attribute)
        {
            return Attribute.IsDefined(info, attribute);
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
