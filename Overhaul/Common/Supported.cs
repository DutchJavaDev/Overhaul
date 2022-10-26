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
        };

        public static PropertyInfo[] GetPropertiesForType(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(i => ValidProperty(i)).ToArray();
        }

        private static bool ValidProperty(PropertyInfo info)
        {
            var read = info.CanRead;
            var write = info.CanWrite;
            var publicSet = info.GetSetMethod(false)?.IsPublic;
            var publicGet = info.GetGetMethod(false)?.IsPublic;

            return read && write &&
                publicSet.Value &&
                publicGet.Value;
        }

        public static string ConvertPropertiesToTypesString(PropertyInfo[] types)
        {
            var names = types.Select(t => t.Name)
                .ToArray();

            var sqlTypes = types.Select(i => SqlTypes[i.PropertyType])
                .ToArray();

            if (names.Length != sqlTypes.Length)
            {
                throw new Exception("Invalid lenght comparison");
            }

            var builder = new StringBuilder();

            for (var i = 0; i < names.Length; i++)
            {
                builder.Append($"{names[i]} {sqlTypes[i]}{(names[i] == names.Last() ? "" : ",")}");
            }

            return builder.ToString();
        }
    } 
}
