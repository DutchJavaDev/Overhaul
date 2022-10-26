namespace Overhaul.Core
{
    internal static class ModelTracker
    {
        private static readonly string _tablePrefix = "tbl";

        public static void Track(IEnumerable<Type> types, string connectionString = "")
        {
            // If debug read connectionstring from secrets
        }

        internal static string GetTableName(Type type)
        {
            return $"{_tablePrefix}{type.Name.ToLower()}";
        }
    }
}
