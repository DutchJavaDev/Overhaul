using Dapper.Contrib.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Overhaul.Common;
using Overhaul.Core;
using Overhaul.Interface;

using OverhaulTests;


namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class SqlModifierTests
    {
        // Model
        private ISqlModifier model;

        public string ConnectionString { get; set; } = string.Empty;

        [TestInitialize]
        public void Init()
        {
            ConnectionString = TestHelper.GetString("devString");

            model = new SqlModifier(ConnectionString);

            ModelTracker.DeleteTestTables(new[] 
            {
               typeof(ModifiedClass),
               typeof(ModifiedClass2)
            },ConnectionString);
        }

        [TestMethod]
        public void AddColumnTest()
        {
            // Arrange
            var originDef = ModelTracker.CreateDefinitions(new[] { typeof(ModifiedClass) })
                .First();

            var newDef = ModelTracker.CreateDefinitions(new[] { typeof(ModifiedClass2) })
                .First();

            var generator = CreateGenerator();

            var result = generator.CreateTable(originDef);

            Assert.IsTrue(result);

            var originColumnArray = Supported.ConvertTypesStringToArray(originDef.ColumnCollection);

            var newColumnArray = Supported.ConvertTypesStringToArray(newDef.ColumnCollection);

            var addColumn = Supported.GetAddedColumns(originColumnArray, newColumnArray)
                .First();

            Assert.AreEqual("Cat FLOAT", addColumn);

            // Act
            result = model.AddColumn(originDef.TableName, addColumn);

            // Assert
            Assert.IsTrue(result);

            generator.DeleteTable(originDef.TableName);
        }


        [TestMethod]
        public void DeleteColumnTest()
        {
            // Arrange
            var originDef = ModelTracker.CreateDefinitions(new[] { typeof(ModifiedClass) })
                .First();

            var newDef = ModelTracker.CreateDefinitions(new[] { typeof(ModifiedClass2) })
                .First();

            var generator = CreateGenerator();

            var result = generator.CreateTable(newDef);

            Assert.IsTrue(result);

            var originColumnArray = Supported.ConvertTypesStringToArray(originDef.ColumnCollection);

            var newColumnArray = Supported.ConvertTypesStringToArray(newDef.ColumnCollection);

            var removedColumn = Supported.GetDeletedColumns(newColumnArray, originColumnArray)
                .First();

            Assert.AreEqual("Cat FLOAT", removedColumn);

            // Act
            result = model.DeleteColumn(originDef.TableName, removedColumn);

            // Assert
            Assert.IsTrue(result);

            generator.DeleteTable(originDef.TableName);
        }

        private ISqlGenerator CreateGenerator()
        {
            return new SqlGenerator(ConnectionString);
        }

        [Table("mold")]
        sealed class ModifiedClass
        {
            [Key]
            public int Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
        }

        [Table("mold")]
        sealed class ModifiedClass2
        {
            [Key]
            public int Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
            public float Cat { get; set; }
        }
    }
}