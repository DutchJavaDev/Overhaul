using Dbhaul.Common;
using Dbhaul.Data;
using System.Text;

namespace Dbhaul.Core
{
    public sealed class PSQLScriptGenerator
    {
        private readonly IList<PSQLModel> _types;

        /// <summary>
        /// This will be running on postgres
        /// </summary>
        /// <param name="types"></param>
        public PSQLScriptGenerator(IList<Type> types)
        {
            _types = DefinitionBuilder.Build(types);
        }

        public string CreateBuildScript()
        {
            var builder = new StringBuilder();

            // create tables with no constraint first
            var orderdTypes = _types.OrderBy(type => type.TableConstraints.Length);

            foreach (var type in orderdTypes)
            {
                builder.AppendLine(DefinitionBuilder.pSQL.CreateTable(type));
                var isLast = type == orderdTypes.Last();
                builder.Append(isLast ? string.Empty : Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
