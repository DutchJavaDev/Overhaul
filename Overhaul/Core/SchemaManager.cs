using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class SchemaManager : ISchemaManager
    {
        private readonly string ConnectionString;
        public SchemaManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void RunSchemaCreate(IEnumerable<TableDefinition> addedTables)
        {
            var queryBuilder = new StringBuilder();

            foreach (var table in addedTables)
            {
                queryBuilder.AppendLine($"{DefaultQuery.CreateTable(table.TableName, table.ColumnCollection)};");
            }

            using var conn = Create();
            conn.ExecuteScalar(queryBuilder.ToString());
        }

        public void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables, 
            IEnumerable<TableDefinition> _cache)
        {
            var queryBuilder = new StringBuilder();

            foreach (var newType in modifiedTables)
            {
                var oldType = _cache.Where(i => i.DefType == newType.DefType
#if DEBUG
               || i.TableName == newType.TableName
#endif
               ).FirstOrDefault();
                
                GetChanges(newType, oldType,
                    out IEnumerable<string> addedColumns,
                    out IEnumerable<string> updatedColumns,
                    out IEnumerable<string> deletedColumns);

                var tasks = new List<Task>();


                using var conn = Create();
                tasks.AddRange(updatedColumns.Select(updatedColumn => Task.Run(() => 
                {
                    conn.ExecuteScalar(DefaultQuery.AlterColumn(newType.TableName, updatedColumn));
                })));

                tasks.AddRange(deletedColumns.Select(deletedColumn => Task.Run(() =>
                {
                    conn.ExecuteScalar(DefaultQuery.DeleteColumn(newType.TableName, deletedColumn));
                })));

                tasks.AddRange(addedColumns.Select(addedColumn => Task.Run(() =>
                {
                    conn.ExecuteScalar(DefaultQuery.AddColumn(newType.TableName, addedColumn));
                })));

                Task.WhenAll(tasks)
                    .Wait();
            }
        }
        public void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables)
        {
            var tasks = new List<Task>();

            using var conn = Create();
            Task.WhenAll(deleteTables.Select(table => Task.Run(() => {
                conn.ExecuteScalar($"DROP TABLE {table.TableName}");
            }))).Wait();
        }

        private static void GetChanges(TableDefinition table, TableDefinition oldType,
            out IEnumerable<string> addedColumns, out IEnumerable<string> updatedColumns, 
            out IEnumerable<string> deletedColumns)
        {
            var oldTypeColumnArray = Supported.ConvertTypesStringToArray(oldType.ColumnCollection);
            var newTypeColumnArray = Supported.ConvertTypesStringToArray(table.ColumnCollection);
            addedColumns = Supported.GetAddedColumns(oldTypeColumnArray, newTypeColumnArray);
            deletedColumns = Supported.GetDeletedColumns(oldTypeColumnArray, newTypeColumnArray);
            updatedColumns = GetUpdatedColumns(addedColumns, deletedColumns);
        }

        private static IEnumerable<string> GetUpdatedColumns(IEnumerable<string> addedColumns, 
            IEnumerable<string> deletedColumns)
        {
            // If they start the same but string length are not equal
            // Might cause a bug :) future me
            var updatedColumns = addedColumns.Where(i => IsAmlostValid(deletedColumns, i));
            addedColumns = addedColumns.Where(i => !updatedColumns.Contains(i));
            deletedColumns = deletedColumns.Where(i => !updatedColumns.Contains(i));
            return updatedColumns;
        }

        private static bool IsAmlostValid(IEnumerable<string> deletedColumns, string addedColumn)
        {
            // Regex Time
            // @"\(.*\)"
            const string patern = @"\(.*\)";

            return deletedColumns.Any(i => 
            {
                var addedMatch = Regex.Match(addedColumn, patern);
                var deletedMatch = Regex.Match(i, patern);

                var addedMinus = addedColumn[..^addedMatch.Value.Length];
                var deletedMinus = i[..^deletedMatch.Value.Length];

                return addedMinus == deletedMinus;
            });
        }

        private SqlConnection Create()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
