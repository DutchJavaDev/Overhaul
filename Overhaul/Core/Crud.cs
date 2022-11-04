﻿using System.Data.SqlClient;

using Dapper;
using Dapper.Contrib.Extensions;

using Overhaul.Data;
using Overhaul.Interface;

namespace Overhaul.Core
{
    internal sealed class Crud : ICrud
    {
        private readonly IEnumerable<TableDefinition> _tableDefCache;
        private readonly string ConnectionString;

        public Crud(IEnumerable<TableDefinition> tables, 
            string connectionString)
        {
            _tableDefCache = tables;
            ConnectionString = connectionString;
        }

        public T Create<T>(T entity) where T : class
        {
            using var conn = Create();
            conn.Insert(entity);
            return entity;
        }

        public void Delete<T>(T entity) where T : class
        {
            using var conn = Create();
            conn.Delete(entity);
        }

        public T Read<T>() where T : class
        {
            using var conn = Create();
            var name = GetTableName(typeof(T));
            return conn.QuerySingle<T>($"SELECT * FROM {name} LIMIT 1");
        }

        public bool Update<T>(T entity) where T : class
        {
            using var conn = Create();
            return conn.Update(entity);
        }

        private SqlConnection Create()
        {
            var sql = new SqlConnection(ConnectionString);
            sql.Open();
            return sql;
        }

        private string GetTableName(Type t)
        {
            var name = _tableDefCache.Where(i => i.DefType == t.Name)
                .FirstOrDefault().TableName;

            if (string.IsNullOrEmpty(name))
            {
                return ModelTracker.GetTableName(t);
            }

            return name;
        }
    }
}