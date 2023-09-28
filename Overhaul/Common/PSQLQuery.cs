using Dbhaul.Core;
using Dbhaul.Data;

namespace Dbhaul.Common
{
    internal static class PSQLQuery
    {
        public static string CreateTable(PSQLModel model)
        {
            return string.Concat("CREATE TABLE IF NOT EXISTS ",model.Name, " (", model.Columns,$"{(model.TableConstraints.Length>0 ? "," : string.Empty)} ",model.TableConstraints,");");
        }
    }
}
