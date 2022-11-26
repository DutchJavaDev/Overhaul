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
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            model = CreateCrudInstance();
            var document = new Document();

            // Act
            model.Create(document);

            // Assert
            Assert.IsNotNull(model.Read<Document>());
            Assert.IsTrue(document.DocumentId > 0);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select(i => new Document())
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var doc = model.GetById<Document>(5);

            // Assert
            Assert.AreEqual(5, doc.DocumentId);
        }
        [TestMethod]
        public void GetByIdColumnsTest()
        {
            // Arrange
            model = CreateCrudInstance();
            var guid = Guid.NewGuid();

            Enumerable.Range(0, 10)
                .Select(i => new Document 
                {
                    Bool = true,
                    Guid = guid
                })
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var doc = model.GetById<Document>(5,
                nameof(Document.Bool),
                nameof(Document.Guid));

            // Assert
            Assert.AreEqual(5, doc.DocumentId);
            Assert.AreEqual(true, doc.Bool);
            Assert.AreEqual(guid, doc.Guid);
        }

        [TestMethod]
        public void GetByTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select((d,index) => new Document
                {
                    String = $"document{index}",
                    Int = index
                })
                .AsParallel()
                .ForAll(d => model.Create(d));

            // Act
            var document = model.GetBy<Document>(nameof(Document.String), "document5");
            var doc2 = model.GetBy<Document>(nameof(Document.Int), 9);

            // Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(doc2);
            Assert.AreEqual("document5", document.String);
            Assert.AreEqual(9, doc2.Int);
        }
        [TestMethod]
        public void GetByColumnTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select((d, index) => new Document
                {
                    String = $"document{index}",
                    Int = index,
                })
                .AsParallel()
                .ForAll(d => model.Create(d));

            // Act
            var document = model.GetBy<Document>(nameof(Document.String),
                "document5", nameof(Document.String));

            var doc2 = model.GetBy<Document>(nameof(Document.Int), 
                9, nameof(Document.Int));

            // Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(doc2);
            Assert.AreEqual("document5", document.String);
            Assert.AreEqual(9, doc2.Int);
        }

        [TestMethod]
        public void GetCollectionTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 50)
                .Select(i => new Document())
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var collection = model.GetCollection<Document>();

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(50, collection.Count());
        }
        [TestMethod]
        public void GetCollectionColumnTest()
        {
            // Arrange
            model = CreateCrudInstance();
            const string str = "this is a string";

            Enumerable.Range(0, 50)
                .Select(i => new Document 
                {
                    String = str
                })
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var collection = model.GetCollection<Document>(nameof(Document.String));

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(50, collection.Count());
            Assert.AreEqual(50, collection.Count(i => i.String.Length == str.Length));
        }

        [TestMethod]
        public void GetCollectionWhereTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select(i => new Document())
                .AsParallel().ForAll(d => model.Create(d));
            Enumerable.Range(0, 10)
                .Select(i => new Document 
                {
                    String = "Hello World"
                })
                .AsParallel().ForAll(d => model.Create(d));
            Enumerable.Range(0, 10)
                .Select(i => new Document 
                {
                    Bool = true
                })
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var collection = model.GetCollectionWhere<Document>(nameof(Document.Bool), true);

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(10, collection.Count());
            Assert.IsTrue(collection.Count(i => i.Bool) == 10);
        }
        [TestMethod]
        public void GetCollectionWhereColumnTest()
        {
            // Arrange
            model = CreateCrudInstance();
            var str = "Yep not Hello world";

            Enumerable.Range(0, 10)
                .Select(i => new Document())
                .AsParallel().ForAll(d => model.Create(d));
            Enumerable.Range(0, 10)
                .Select(i => new Document
                {
                    String = "Hello World"
                })
                .AsParallel().ForAll(d => model.Create(d));
            Enumerable.Range(0, 10)
                .Select(i => new Document
                {
                    Bool = true,
                    String = str,
                    Int = 17
                })
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var collection = model.GetCollectionWhere<Document>(nameof(Document.Bool), 
                true, nameof(Document.String), nameof(Document.Bool));

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(10, collection.Count());
            Assert.IsTrue(collection.Count(i => i.Bool) == 10);
            Assert.IsTrue(collection.Count(i => i.Int == 0) == 10);
            Assert.IsTrue(collection.Count(i => i.String == str) == 10);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            // Arrange
            model = CreateCrudInstance();
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
            model = CreateCrudInstance();
            var document = new Document();

            model.Create(document);

            // Act
            model.Delete(document);

            // Assert
            Assert.IsNull(model.Read<Document>());
        }

        private IModelTracker CreateModelTracker()
        {
            ModelTracker.DeleteTestTables(Types, ConnectionString);

            IModelTracker tracker = new ModelTracker(ConnectionString);
            tracker.Track(Types);
            return tracker;
        }

        private ICrud CreateCrudInstance()
        {
            return CreateModelTracker()
                .GetCrudInstance();
        }

        [Table("tblSecreteDocumentsForTest")]
        public sealed class Document
        {
            [Key]
            public int DocumentId { get; set; }
            public int DocumentNumber { get; set; }
            public int Int { get; set; }
            public string String { get; set; } = string.Empty;
            public float? Float { get; set; }
            public decimal Decimal { get; set; }
            public char Char { get; set; }
            public double DDouble { get; set; }
            public Guid Guid { get; set; }
            public short? Short { get; set; }
            public byte Byte { get; set; }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now;
        }
    }
}