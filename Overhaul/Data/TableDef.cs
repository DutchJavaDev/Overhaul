namespace Overhaul.Data
{
    internal sealed class TableDef
    {
        public Guid Id { get; set; }
        public string TableName { get; set; }
        public string DefType { get; set; }
        public string ColumnCollection { get; set; }
        public int ColumnCount { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TableDef other)
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

        private bool Equals(TableDef other)
        {
            return SameTableName(other.TableName)
                && SameDefType(other.DefType)
                && SameColumnCollection(other.ColumnCollection,
                other.ColumnCount);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
