using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Data;
using Overhaul.Interface;
using OverhaulTests;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public sealed class SqlGeneratorTests
    {
        private ISqlGenerator model { get; set; }

        private static Type Type = typeof(TableClass);

        [TestInitialize]
        public void Init()
        {
            var connection = TestHelper.GetString("devString");

            model = new SqlGenerator(connection);

            ModelTracker.DeleteTestTables(new[] { Type }, connection);
        }

        // exclude until fixed
        //[TestMethod]
        public void BGetcollectionTest()
        {
            // Act
            var result = model.GetCollection();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ACreateTableTest()
        {
            // Arrange
            var def = CreateDef();

            // Act
            var result = model.CreateTable(def);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CDeleteTableTest()
        {
            // Arrange
            var tableName = ModelTracker.GetTableName(typeof(TableClass));

            // Act
            var result = model.DeleteTable(tableName);

            // Assert
            Assert.IsTrue(result);
        }

        private static TableDefinition CreateDef()
        {
            var def = ModelTracker.CreateDefinitions(new[] { Type })
                .First();
            return def;
        }

        [Table("tblTblaClass")]
        public sealed class TableClass
        {
            public string Name { get; set; } = string.Empty;
            public int Number { get; set; }
            public Guid Description { get; set; }
        }
    }
}