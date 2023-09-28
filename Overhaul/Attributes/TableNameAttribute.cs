using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbhaul.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableNameAttribute : Attribute
    {
        public string TableName { get; init; }
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
