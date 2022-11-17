using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Data.Attributes;
using Overhaul.Interface;
using OverhaulTests;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class SchemaManagerTests
    {
        // Model
        private ISchemaManager model;

        // Parameters
        private ISqlGenerator sqlGenerator;
        private ISqlModifier sqlModifier;
        private const string TableName = "mod";
         
        [TestInitialize]
        public void Init()
        {
            var connection = TestHelper.GetString("devString");

            sqlGenerator = new SqlGenerator(connection);
            sqlModifier = new SqlModifier(connection);

            model = new SchemaManager(sqlGenerator,sqlModifier);

            sqlGenerator.DeleteTable(TableName);

            ModelTracker.Track(Enumerable.Empty<Type>(), connection);
        }

        [TestMethod()]
        public void RunSchemaCreateTest()
        {
            // Arrange
            var types = CreateDefinitions();

            // Act
            model.RunSchemaCreate(types);

            // Assert
            Assert.IsTrue(sqlGenerator.TableExists(TableName));
        }

        [TestMethod()]
        public void RunSchemaUpdateTest()
        {
            // Arrange
            var types = CreateDefinitions();
            var types2 = CreateDefinitions2();

            // Act
            model.RunSchemaCreate(types);
            model.RunSchemaUpdate(types2, types);

            // Assert
            Assert.IsTrue(sqlGenerator.TableExists(TableName));
        }

        [TestMethod()]
        public void RunSchemaDeleteTest()
        {
            // Arrange
            var types = CreateDefinitions();
            var types2 = CreateDefinitions2();
            var types3 = CreateDefinitions3();

            // Act
            model.RunSchemaCreate(types);
            model.RunSchemaUpdate(types2, types);
            model.RunSchemaUpdate(types3, types2);
            model.RunSchemaDelete(types3);

            // Assert
            Assert.IsTrue(!sqlGenerator.TableExists(TableName));
        }

        private IEnumerable<TableDefinition> CreateDefinitions()
        {
             var types = new[] 
             { 
                 typeof(ModifiedClass),
             };

            return ModelTracker.CreateDefinitions(types);
        }
        private IEnumerable<TableDefinition> CreateDefinitions2()
        {
            var types = new[]
            {
                 typeof(ModifiedClass2),
             };

            return ModelTracker.CreateDefinitions(types);
        }
        private IEnumerable<TableDefinition> CreateDefinitions3()
        {
            var types = new[]
            {
                 typeof(ModifiedClass3),
             };

            return ModelTracker.CreateDefinitions(types);
        }

        [Table(TableName)]
        sealed class ModifiedClass
        {
            public string Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
        }

        [Table(TableName)]
        sealed class ModifiedClass2
        {
            public string Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
            public float Cat { get; set; }
        }

        [Table(TableName)]
        sealed class ModifiedClass3
        {
            public string Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
            public float Cat { get; set; }
            [Precision(15.5)]
            public decimal Cash { get; set; }
        }
    }
}