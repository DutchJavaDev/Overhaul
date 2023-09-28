using Dbhaul.Common;
using Dbhaul.Core;

namespace Dbhaul.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OneToMany : Attribute
    {
        public string Constraint { get; init; }

        public OneToMany(Type type, string keyName)
        {
            var identityProperty = DefinitionBuilder.GetIdentityType(type);
            var tableName = DefinitionBuilder.GetName(type);
            Constraint = $"constraint fk_{tableName} foreign key({keyName}) references {tableName}({identityProperty.Name})";
        }
    }
}
