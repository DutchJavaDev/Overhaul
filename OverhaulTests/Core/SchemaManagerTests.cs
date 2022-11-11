using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Common;
using Overhaul.Data;
using Overhaul.Data.Attributes;
using Overhaul.Interface;
using OverhaulTests;
using System.Runtime.CompilerServices;

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

        [TestInitialize]
        public void Init()
        {
            var connection = TestHelper.GetString("devString");

            sqlGenerator = new SqlGenerator(connection);
            sqlModifier = new SqlModifier(connection);

            model = new SchemaManager(sqlGenerator,sqlModifier);
        }

        [TestMethod()]
        public void RunSchemaCreateTest()
        {
            // Arrange
            var types = CreateDefinitions();

            // Act
            model.RunSchemaCreate(types);
        }

        [TestMethod()]
        public void RunSchemaUpdateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RunSchemaDeleteTest()
        {
            Assert.Fail();
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

        [Table("mod")]
        sealed class ModifiedClass
        {
            public string Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
        }

        [Table("mod")]
        sealed class ModifiedClass2
        {
            public string Id { get; set; }
            public int Bingo { get; set; }
            public bool BitSet { get; set; }
            public float Cat { get; set; }
        }

        [Table("mod")]
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