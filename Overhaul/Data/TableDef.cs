namespace Overhaul.Data
{
    internal sealed class TableDef
    {
        public string TableName { get; set; }
        public string DefType { get; set; }
        public string ColumnCollection { get; set; }

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

        private bool SameColumnCollection(string other)
        {
            return ColumnCollection.Equals(other);
        }

        private bool Equals(TableDef other)
        {
            return SameTableName(other.TableName)
                && SameDefType(other.DefType)
                && SameColumnCollection(other.ColumnCollection);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
