using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class ModelTrackerTests
    {
        [TestMethod()]
        public void TrackTest()
        {
            Assert.IsTrue(true);
        }

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

            Assert.AreEqual(noAttr.TableName, "tblNoTableAttributeClass");
            Assert.AreEqual(attr.TableName, "tblcustomName");

            Assert.AreEqual(noAttr.ColumnCollection, string.Empty);
            Assert.AreEqual(attr.ColumnCollection, string.Empty);

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
            Assert.AreEqual("tblcustomName",tableName);
        }

        [TestMethod]
        public void GeTableTableNameTest()
        {
            // Arrane
            var _type = typeof(NoTableAttributeClass);

            // Act
            var tableName = ModelTracker.GetTableName(_type);

            // Assert
            Assert.AreEqual("tblNoTableAttributeClass", tableName);
        }

        private sealed class NoTableAttributeClass 
        { }

        [Table("customName")]
        private sealed class TableAttributeClass
        { }
    }
}