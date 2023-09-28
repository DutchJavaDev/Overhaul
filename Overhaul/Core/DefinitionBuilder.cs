using Dbhaul.Attributes;
using Dbhaul.Common;
using Dbhaul.Data;
using System.Reflection;
using System.Text;

namespace Dbhaul.Core
{
    public static class DefinitionBuilder
    {
        public static IList<PSQLModel> Build(IList<Type> models)
        {
            return models.Select(i => {

                CacheType(i);

                return new PSQLModel
                {
                    Name = GetName(i),
                    Columns = GetColumns(i),
                    TableConstraints = GetConstrainst(i)
                };

            }).ToList();
        }

        private static void CacheType(Type type)
        {
            PropertyInfoCache.Add(type.GUID, type.GetProperties());
            AttributeCache.Add(type.GUID, type.GetCustomAttributes());
        }

        private static string GetColumns(Type model)
        {
            var properties = GetTypeProperties(model);
            return PSQLSupported.ConvertPropertiesToTypesString(properties);
        }

        private static string GetConstrainst(Type model)
        {
            if (HasOneToManyConstraint(model, out var constring))
            {
                return constring;
            }
            return string.Empty;
        }

        private static bool HasOneToManyConstraint(Type model, out string constring)
        {
            var has = IsDefined(model, typeof(OneToMany));

            if (has)
            {
                var builder = new StringBuilder();
                var oneToManys= AttributeCache.Get(model.GUID).
                    Where(i => i.GetType() == typeof(OneToMany))
                    .Select(i => (OneToMany) i);

                foreach (var otm in oneToManys)
                {
                    builder.AppendLine(otm.Constraint);
                }

                constring = builder.ToString();
                return has;
            }

            constring = string.Empty;
            return has;
        }

        private static bool IsDefined(MemberInfo info, Type attribute)
        {
            return Attribute.IsDefined(info, attribute) || info.CustomAttributes.Any(i => i.GetType() == attribute);
        }

        public static string GetName(Type model)
        {
            var (hasValue, table) = HasTableAttribute(model);
            if (hasValue)
            {
                return table.TableName;
            }
            return model.Name;
        }

        public static (bool hasValue, TableNameAttribute table) HasTableAttribute(Type model)
        {
            if (Attribute.IsDefined(model, typeof(TableNameAttribute))
                && AttributeCache.Get(model.GUID).Where(i => i .GetType() == typeof(TableNameAttribute))
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
