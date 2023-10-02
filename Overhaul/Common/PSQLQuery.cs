using Dbhaul.Attributes;
using Dbhaul.Core;
using Dbhaul.Data;
using System.Text;

namespace Dbhaul.Common
{
    public sealed class PSQLQuery
    {
        private readonly string fKTPPH = "{x}";
        private readonly string createTableNotExists = "create table if not exists ";
        private readonly string constrainPrefix = "constraint fk_";
        private readonly string foreingKeyTemplate = "foreign key({x})";

        public string CreateTable(PSQLModel model)
        {
            var builder = new StringBuilder();
            var hasConstraints = model.TableConstraints.Length > 0;

            builder.AppendLine(string.Concat(createTableNotExists, model.Name, " ("));

            if (hasConstraints)
            {
                builder.AppendLine($"{model.Columns},");
                builder.Append(model.TableConstraints);
                builder.AppendLine(");");
            }
            else
            {
                builder.AppendLine(model.Columns);
                builder.Append(");");
            }
            return builder.ToString();
        }

        public string OneToMany(OneToMany oneToMany)
        {
            var identityProperty = DefinitionBuilder.GetIdentityType(oneToMany.Type);
            var tableName = DefinitionBuilder.GetName(oneToMany.Type);
            var keyName = oneToMany.KeyName;
            var temp = string.Concat(constrainPrefix,tableName," ",foreingKeyTemplate.Replace(fKTPPH, keyName));
            return $"constraint fk_{tableName} foreign key({keyName}) references {tableName}({identityProperty.Name})";
        }

        public string ManyToMany(Type origin, ManyToMany manyToMany)
        {
            var originName = DefinitionBuilder.GetName(origin);
            var targetName = DefinitionBuilder.GetName(manyToMany.Type);

            var tableName = string.Concat(originName, '_', targetName);

            var originId = string.Concat(originName, "_id");
            var targetId = string.Concat(targetName, "_id");

            var originConstraintName = string.Concat("fk_", originName);
            var targetConstraintName = string.Concat("fk_", targetName);

            var originIdentityType = DefinitionBuilder.GetIdentityType(origin);
            var targetIdentityType = DefinitionBuilder.GetIdentityType(manyToMany.Type);

            var columns = PSQLSupported.ConvertPropertiesToTypesString(new[] { originIdentityType, targetIdentityType });

            columns = columns.Replace(originIdentityType.Name, originId);
            columns = columns.Replace(targetIdentityType.Name, targetId);

            var builder = new StringBuilder();

            builder.AppendLine(string.Concat("create table ", tableName, " ("));
            builder.AppendLine(columns);
            builder.AppendLine(string.Concat("primary key (", originId, ',', targetId, ')'));
            builder.AppendLine(string.Concat("constraint ", originConstraintName, " foreign key(", originId, ") references ", originName, '(', originId, ')'));
            builder.AppendLine(string.Concat("constraint ", targetConstraintName, " foreign key(", targetId, ") references ", targetName, '(', targetId, ')'));
            builder.Append(')');

            return builder.ToString();
        }
    }
}
