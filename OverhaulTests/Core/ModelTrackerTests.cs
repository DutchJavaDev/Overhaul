using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverhaulTests;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class ModelTrackerTests
    {
        [TestMethod]
        public void CreateDefenitionsTest()
        {
            // Arrange
            var types = new[]
            {
                typeof(NoTableAttributeClass),
                typeof(TableAttributeClass)
            };

            // Act
            var definitons = ModelTracker.CreateDefinitions(types);

            // Assert
            var noAttr = definitons.Where(i => i.DefType.Equals(types[0].Name))
                .First();
            var attr = definitons.Where(i => i.DefType.Equals(types[1].Name))
                .First();

            Assert.AreEqual(noAttr.TableName, "NoTableAttributeClass");
            Assert.AreEqual(attr.TableName, "customName");

            Assert.AreEqual("Name FLOAT", noAttr.ColumnCollection);
            Assert.AreEqual("Age INT", attr.ColumnCollection);

            Assert.AreEqual(noAttr.DefType, types[0].Name);
            Assert.AreEqual(attr.DefType, types[1].Name);
        }

        [TestMethod]
        public void GeTableAttributetTableNameTest()
        {
            // Arrange
            var _type = typeof(TableAttributeClass);

            // Act
            var tableName = ModelTracker.GetTableName(_type);

            // Assert
            Assert.AreEqual("customName", tableName);
        }

        [TestMethod]
        public void GeTableTableNameTest()
        {
            // Arrange
            var _type = typeof(NoTableAttributeClass);

            // Act
            var tableName = ModelTracker.GetTableName(_type);

            // Assert
            Assert.AreEqual("NoTableAttributeClass", tableName);
        }

        [TestCleanup]
        public void CleanUp()
        {
            ModelTracker.DeleteTestTables(new[] { 
                typeof(NoTableAttributeClass),
                typeof(TableAttributeClass),
            }, TestHelper.GetString("devString"));
        }

        private sealed class NoTableAttributeClass 
        {
            public double Name { get; set; }
        }

        [Table("customName")]
        private sealed class TableAttributeClass
        {
            public int Age { get; set; }
        }
    }
}