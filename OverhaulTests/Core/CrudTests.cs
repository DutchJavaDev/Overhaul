using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Interface;
using OverhaulTests;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class CrudTests
    {
        // Model
        private ICrud model;

        // Helpers
        private readonly Type[] Types = new[] { typeof(Document) };
        private string ConnectionString;

        [TestInitialize]
        public void Init()
        {
            ConnectionString = TestHelper.GetString("devString");

            ModelTracker.DeleteTestTables(Types, ConnectionString);

            ModelTracker.Track(Types, ConnectionString);

            model = ModelTracker.GetCrudInstance();
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            var document = new Document();

            // Act
            model.Create(document);

            // Assert
            Assert.IsNotNull(model.Read<Document>());
            Assert.IsTrue(document.DocumentId > 0);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            // Arrange
            var document = new Document();

            model.Create(document);

            // Arrange
            document.Bool = true;

            // Act
            model.Update(document);

            // Assert
            var updModel = model.Read<Document>();
            Assert.AreEqual(true, updModel.Bool);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Arrange
            ModelTracker.DeleteTestTables(Types, ConnectionString);

            ModelTracker.Track(Types, ConnectionString);

            var document = new Document();

            model.Create(document);

            // Act
            model.Delete(document);

            // Assert
            Assert.IsNull(model.Read<Document>());
        }


        [Table("tblSecreteDocumentsForTest")]
        public sealed class Document
        {
            [Key]
            public int DocumentId { get; set; }
            public int DocumentNumber { get; set; }
            public int Int { get; set; }
            public string String { get; set; } = string.Empty;
            public float Float { get; set; }
            public decimal Decimal { get; set; }
            public char Char { get; set; }
            public double DDouble { get; set; }
            public Guid Guid { get; set; }
            public short Short { get; set; }
            public byte Byte { get; set; }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now;
        }
    }
}