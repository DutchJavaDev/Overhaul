using Dbhaul.Attributes;
using Dbhaul.Core;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
///BASED OF SQL
[assembly: InternalsVisibleTo("DbhaulTests")]
namespace Dbhaul.Common
{
    internal static partial class PSQLSupported
    {
        public readonly static Dictionary<Type, string> PSQLTypes = new()
        {
            {typeof(int), "integer"},
            {typeof(double), "double precision"},
            {typeof(string), "text"},
            {typeof(bool), "bit" },
            {typeof(Guid), "uuid "},
            {typeof(float), "real" },
            {typeof(byte), "smallint" },
            {typeof(short), "smallint" },
            {typeof(decimal), "numeric" },
            {typeof(DateTime), "date"},
        };

        public readonly static List<Type> IdentityTypes = new()
        {
            typeof(int),
            typeof(byte),
            typeof(long)
        };

        public static string ConvertPropertiesToTypesString(PropertyInfo[] types)
        {
            var validColumns = types.Where(i => !i.CustomAttributes
                .Any(i => i.GetType() == typeof(IgnorePropertyAttribute)))
                .Select(GetColumnDefinition);

            var builder = new StringBuilder();

            foreach(var column in validColumns)
            {
                builder.Append($"{column}{(column == validColumns.Last() ? "" : ",")}");
            }

            return builder.ToString();
        }
        public static string[] ConvertTypesStringToArray(string types)
        {
            return types.Split(";").Select(i => i.Trim()).ToArray();
        }

        public static (bool hasValue, TableNameAttribute table) HasTableAttribute(Type model)
        {
            if(Attribute.IsDefined(model, typeof(TableNameAttribute))
                && model.GetCustomAttribute(typeof(TableNameAttribute))
                is TableNameAttribute table)
            {
                return (true, table);
            }

            return (false, null);
        }

        internal static bool ValidProperty(PropertyInfo info)
        {
            // Ignore
            if (info.CustomAttributes.Any(i => typeof(IgnorePropertyAttribute) == i.AttributeType))
            {
                return false;
            }

            var read = info.CanRead;
            var write = info.CanWrite;
            //var primitieve = info.DeclaringType.IsPrimitive; does not support datetime && guid
            var publicSet = info.GetSetMethod(false)?.IsPublic;
            var publicGet = info.GetGetMethod(false)?.IsPublic;

            return read && write &&
                publicSet.Value &&
                publicGet.Value;
        }


        private static IEnumerable<T> GetDifferenceFromCollections<T>(IEnumerable<T> source, IEnumerable<T> target)
        {
            return source.Where(i => !target.Contains(i));
        }

        private static string GetColumnDefinition(PropertyInfo property)
        {
            Type propertyType = GetType(property);

            var buffer = new StringBuilder();
            buffer.Append(property.Name);
            buffer.Append(' ');
            buffer.Append(PSQLTypes[propertyType]);

            if (IsIdentityType(property))
            {
                buffer.Append(" generated always as identity ");
            }
            else if (IsIdentityTypeGUID(property))
            {
                buffer.Append(" default gen_random_uuid() primary key ");
            }
            else if (IsDefined(property, typeof(PrecisionAttribute))
                && property.GetCustomAttribute<PrecisionAttribute>() is
                PrecisionAttribute precision)
            {
                buffer.Append(GetPrecision(precision));
            }

            return buffer.ToString();
        }

        public static PropertyInfo GetIdentityType(Type type)
        {
            return PropertyInfoCache.Get(type.GUID).Where(i => IsIdentityType(i) || IsIdentityTypeGUID(i))
                .FirstOrDefault();
        }

        private static Type GetType(PropertyInfo property)
        {
            Type _type;
            if (property.PropertyType.IsGenericType
                            && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                _type = Nullable.GetUnderlyingType(property.PropertyType);
            }
            else if (property.PropertyType.IsEnum)
            {
                _type = typeof(byte);
            }
            else
            {
                _type = property.PropertyType;
            }

            return _type;
        }

        private static string GetPrecision(PrecisionAttribute pa)
        {
            return $"({pa.Precision})";
        }

        public static bool IsIdentityType(PropertyInfo type)
        {
            return IsDefined(type, typeof(IdAttribute))
                && IdentityTypes.Contains(ResolvePropertyType(type.PropertyType));
        }

        public static bool IsIdentityTypeGUID(PropertyInfo type)
        {
            return IsDefined(type, typeof(IdAttribute))
                && typeof(Guid) == ResolvePropertyType(type.PropertyType);
        }

        private static bool IsDefined(MemberInfo info, Type attribute)
        {
            return Attribute.IsDefined(info, attribute) || info.CustomAttributes.Any(i => i.GetType() == attribute);
        }


        public static Type ResolvePropertyType(Type type)
        {
            if (type.Name.Contains("Nullable`1"))
            {
                return Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        [GeneratedRegex("[1-9]{1,}")]
        private static partial Regex ForgotWhatThisDoesRegex();
    }
}
