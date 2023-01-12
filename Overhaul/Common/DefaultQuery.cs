using Overhaul.Core;

namespace Overhaul.Common
{
    internal static class DefaultQuery
    {
        public static string AddColumn(string tableName, string column)
        {
            return $"ALTER TABLE {tableName} ADD {column}";
        }
        public static string DeleteColumn(string tableName, string column)
        {
            var query = $"ALTER TABLE {tableName} ";

            if (ModelTracker.Options.DataLose)
            {
                query += $"DROP DOLUMN {column}";
            }
            else
            {
                query += $"ALTER COLUMN {column}" +
                $"{(column.Contains("NULL") ? "" : " NULL")}";
            }

           return query;
        }
        public static string AlterColumn(string tableName, string column)
        {
            return $"ALTER TABLE {tableName} ALTER COLUMN {column}";
        }
        public static string CreateTable(string tableName, string columns)
        {
            return $"CREATE TABLE {tableName} " +
               $"({columns.Replace(";", ",")})";
        }
        public static string Select(string columns, string tableName)
        {
            return $"SELECT {columns} " +
                    $"FROM {tableName}";
        }
        public static string Top1(string columns, string tableName)
        {
            return $"SELECT TOP 1 {columns} FROM {tableName}";
        }
        public static string GetBy(string columns, string tableName, string columnName)
        {
            return $"SELECT {columns} FROM {tableName} " +
                $"WHERE {columnName} = @value"; ;
        }

        public static string GetById(string columns, string keyName, string tableName)
        {
            return $"SELECT {columns},{keyName} FROM " +
                          $"{tableName} " +
                          $"WHERE {keyName} = @id";
        }
    }
}
