﻿using Dapper.Contrib.Extensions;

using Overhaul.Data.Attributes;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Common
{
    internal static class Supported
    {
        private readonly static Dictionary<Type, string> DefaultTypes = new()
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
            {typeof(char), "CHAR" }
        };

        private readonly static List<Type> IdentityTypes = new()
        {
            typeof(int),
            typeof(Guid),
        };

        public static PropertyInfo[] GetTypeProperties(Type type)
        {
            return type.GetProperties().Where(i => ValidProperty(i))
                .ToArray();
        }

        public static string ConvertPropertiesToTypesString(PropertyInfo[] types, out int count)
        {
            var validColumns = types.Where(i => !i.CustomAttributes
                .Any(i => i.GetType() == typeof(ComputedAttribute)))
                .Select(i => GetColumnDefinition(i));

            var builder = new StringBuilder();

            count = 0;

            foreach(var column in validColumns)
            {
                builder.Append($"{column}{(column == validColumns.Last() ? "" : ";")}");
                count++;
            }

            return builder.ToString();
        }

        public static string[] ConvertTypesStringToArray(string types)
        {
            return types.Split(";").Select(i => i.Trim()).ToArray();
        }

        public static IEnumerable<string> GetAddedColumns(string[] self, string[] other)
        {
            return GetDifferenceFromCollections(other, self);
        }

        public static IEnumerable<string> GetDeletedColumns(string[] self, string[] other)
        {
            return GetDifferenceFromCollections(self, other);
        }

        internal static bool ValidProperty(PropertyInfo info)
        {
            // Ignore
            if (info.CustomAttributes.Any(i => typeof(ComputedAttribute) == i.AttributeType))
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
            Type type = GetType(property);

            var name = property.Name;
            var column = DefaultTypes[type];

            if (IsIdentityType(property))
            {
                column += " IDENTITY(1,1) ";
            }

            if (IsDefined(property, typeof(PrecisionAttribute))
                && property.GetCustomAttribute<PrecisionAttribute>() is
                PrecisionAttribute precision)
            {
                column = GetPrecision(column, precision);
            }

            return $"{name} {column}";
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
                // for now
                _type = typeof(byte);
            }
            else
            {
                _type = property.PropertyType;
            }

            return _type;
        }

        private static string GetPrecision(string column, PrecisionAttribute pa)
        {
            if (column.Contains('('))
            {
                column = Regex.Replace(column, "[1-9]{1,}", pa.Precision);
            }
            else
            {
                column += $"({pa.Precision})";
            }

            return column;
        }

        private static bool IsIdentityType(PropertyInfo type)
        {
            return IsDefined(type, typeof(KeyAttribute))
                && IdentityTypes.Contains(type.PropertyType);
        }

        private static bool IsDefined(MemberInfo info, Type attribute)
        {
            return Attribute.IsDefined(info, attribute);
        }

    }
}
