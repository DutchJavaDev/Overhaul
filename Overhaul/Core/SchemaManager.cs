using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Z.Dapper.Plus;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Interface;
using Dapper.Contrib.Extensions;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("OverhaulTests")]
namespace Overhaul.Core
{
    internal sealed class SchemaManager : ISchemaManager
    {
        private static string ConnectionString;
        public SchemaManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void RunSchemaCreate(IEnumerable<TableDefinition> addedDefinitions)
        {
            var queryBuilder = new StringBuilder();

            foreach (var table in addedDefinitions)
            {
                queryBuilder.AppendLine($"{DefaultQuery.CreateTable(table.TableName, table.ColumnCollection)};");
            }

            using var conn = Create();
            conn.ExecuteScalar(queryBuilder.ToString());

            InsertAddedDefinitions(addedDefinitions);
        }

        public void RunSchemaUpdate(IEnumerable<TableDefinition> modifiedTables, 
            IEnumerable<TableDefinition> _cache)
        {
            var queryBuilder = new StringBuilder();
            var tasks = new List<Task>();
            var queries = new List<string>();

            using var conn = Create();
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

                UpdateColumns(updatedColumns, oldType, newType);
                DeleteColumns(deletedColumns, oldType, newType);
                AddedColumns(addedColumns, oldType, newType);
            }
        }
        public void RunSchemaDelete(IEnumerable<TableDefinition> deleteTables)
        {
            const string ifQuery = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName)";
            
            using var conn = Create();
            foreach (var table in deleteTables)
            {
                conn.ExecuteScalar($"{ifQuery} DROP TABLE {table.TableName}", new
                {
                    tableName = table.TableName
                });    
            }
        }

        private static void UpdateColumns(IEnumerable<string> columns,
            TableDefinition oldType, TableDefinition newType)
        {
            using var conn = Create();
            foreach (var column in columns)
            {
                var query = DefaultQuery.AlterColumn(newType.TableName, column);
                conn.ExecuteScalar(query);
                newType.Id = oldType.Id;
                conn.Update(newType);
            }
        }

        private static void DeleteColumns(IEnumerable<string> columns,
            TableDefinition oldType, TableDefinition newType)
        {
            using var conn = Create();
            foreach (var column in columns)
            {
                var query = DefaultQuery.DeleteColumn(newType.TableName, column);
                conn.ExecuteScalar(query);
                // dont update, column is set to be nullable
                //newType.Id = oldType.Id;
                //conn.Update(newType);
            }
        }

        private static void AddedColumns(IEnumerable<string> columns,
            TableDefinition oldType, TableDefinition newType)
        {
            using var conn = Create();
            foreach (var column in columns)
            {
                var query = DefaultQuery.AddColumn(newType.TableName, column);
                conn.ExecuteScalar(query);
                newType.Id = oldType.Id;
                conn.Update(newType);
            }
        }

        private void InsertAddedDefinitions(IEnumerable<TableDefinition> tableDefinitions)
        {
            using var conn = Create();
            conn.BulkInsert(tableDefinitions);
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

        private static SqlConnection Create()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
