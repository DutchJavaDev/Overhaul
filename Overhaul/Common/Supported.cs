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

        public static PropertyInfo[] GetPropertiesForType(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(i => ValidProperty(i)).ToArray();
        }

        public static string ConvertPropertiesToTypesString(PropertyInfo[] types)
        {
            var validTypes = types.Where(i => SqlTypes.ContainsKey(i.PropertyType))
                .Select(i => (i.Name, SqlTypes[i.PropertyType]));

            var builder = new StringBuilder();

            foreach(var _type in validTypes)
            {
                builder.Append($"{_type.Name} {_type.Item2}{(_type == validTypes.Last() ? "" : ",")}");
            }

            return builder.ToString();
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
