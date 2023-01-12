using Dapper.Contrib.Extensions;

namespace Overhaul.Data
{
    [Table("tblDefinition")]
    internal sealed class TableDefinition
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; } 
        public string DefType { get; set; }
        public string ColumnCollection { get; set; }
        public int ColumnCount { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TableDefinition other)
            {
                return Equals(other);
            }

            return false;
        }

        private bool SameTableName(string other)
        {
            return TableName.Equals(other);
        }

        private bool SameDefType(string other)
        {
            return DefType.Equals(other);
        }

        private bool SameColumnCollection(string other, int count)
        {
            return ColumnCount == count 
                && ColumnCollection.Equals(other);
        }

        private bool Equals(TableDefinition other)
        {
            var tabelName = SameTableName(other.TableName);
             var defType = SameDefType(other.DefType);
             var columnCount = SameColumnCollection(other.ColumnCollection,
                other.ColumnCount);
            return tabelName && defType && columnCount;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 17;
        }
    }
}
