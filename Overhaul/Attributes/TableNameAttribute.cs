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
