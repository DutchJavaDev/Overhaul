using Dbhaul.Common;
using Dbhaul.Data;
using Dbhaul.Interface;
using System.Text;

namespace Dbhaul.Core
{
    public sealed class PSQLScriptGenerator : IScriptGenerator
    {
        private readonly IList<PSQLModel> _models;

        /// <summary>
        /// This will be running on postgres
        /// </summary>
        /// <param name="models"></param>
        public PSQLScriptGenerator(IList<Type> models)
        {
            _models = DefinitionBuilder.Build(models);
        }

        public string CreateBuildScript()
        {
            var builder = new StringBuilder();

            // Create tables if not exists
            foreach (var model in _models)
            {
                builder.AppendLine(PSQLQuery.CreateTable(model));
            }

            return builder.ToString();
        }
    }
}
