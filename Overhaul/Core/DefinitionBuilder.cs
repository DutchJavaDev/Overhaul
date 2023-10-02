using Dbhaul.Attributes;
using Dbhaul.Common;
using Dbhaul.Data;
using System.Data;
using System.Reflection;
using System.Text;

namespace Dbhaul.Core
{
    public static class DefinitionBuilder
    {
        public static readonly PSQLQuery pSQL = new();

        public static IList<PSQLModel> Build(IList<Type> types)
        {
            CacheTypes(types);

            return types.Select(type =>
            {
                return CreateModel(type);

            }).ToList();
        }

        private static PSQLModel CreateModel(Type type)
        {
            return new PSQLModel
            {
                Name = GetName(type),
                Columns = GetColumns(type),
                TableConstraints = GetConstrainst(type)
            };
        }

        private static void CacheTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                PropertyInfoCache.Add(type.GUID, type.GetProperties());
                AttributeCache.Add(type.GUID, type.GetCustomAttributes());
            }
        }

        private static string GetColumns(Type type)
        {
            var typeProperties = GetTypeProperties(type);
            return PSQLSupported.ConvertPropertiesToTypesString(typeProperties);
        }

        private static string GetConstrainst(Type type)
        {
            var builder = new StringBuilder();

            builder.Append(GetOneToManyConstraints(type));
            builder.AppendLine(GetManyToManyConstraints(type));

            return builder.ToString();
        }
        
        private static string GetOneToManyConstraints(Type type)
        {
            if (IsDefined(type, typeof(OneToMany)))
            {
                var builder = new StringBuilder();
                var oneToManys = AttributeCache.Get(type.GUID).
                    Where(i => i.GetType() == typeof(OneToMany))
                    .Select(i => (OneToMany)i);

                foreach (var oneToMany in oneToManys)
                {
                    builder.AppendLine(pSQL.OneToMany(oneToMany));
                }

                return builder.ToString();
            }

            return string.Empty;
        }

        private static string GetManyToManyConstraints(Type type)
        {
            if (IsDefined(type, typeof(ManyToMany)))
            {
                var builder = new StringBuilder();
                var manyToManys = AttributeCache.Get(type.GUID).
                    Where(i => i.GetType() == typeof(ManyToMany))
                    .Select(i => (ManyToMany)i);

                foreach (var manyToMany in manyToManys)
                {
                    builder.AppendLine(pSQL.ManyToMany(type,manyToMany));
                }

                return builder.ToString();
            }

            return string.Empty;
        }

        private static bool IsDefined(MemberInfo info, Type attribute)
        {
            return Attribute.IsDefined(info, attribute) || info.CustomAttributes.Any(i => i.GetType() == attribute);
        }

        public static string GetName(Type type)
        {
            var (hasValue, table) = HasTableAttribute(type);
            if (hasValue)
            {
                return table.TableName;
            }
            return type.Name;
        }

        public static (bool hasValue, TableNameAttribute table) HasTableAttribute(Type type)
        {
            if (Attribute.IsDefined(type, typeof(TableNameAttribute))
                && AttributeCache.Get(type.GUID).Where(i => i.GetType() == typeof(TableNameAttribute))
                .FirstOrDefault()
                is TableNameAttribute table)
            {
                return (true, table);
            }

            return (false, null);
        }

        public static PropertyInfo[] GetTypeProperties(Type type)
        {
            return PropertyInfoCache.Get(type.GUID).Where(i => PSQLSupported.ValidProperty(i))
                .ToArray();
        }

        public static PropertyInfo GetIdentityType(Type type)
        {
            return PropertyInfoCache.Get(type.GUID).Where(i => PSQLSupported.IsIdentityType(i)
            || PSQLSupported.IsIdentityTypeGUID(i))
                .FirstOrDefault();
        }

    }
}
