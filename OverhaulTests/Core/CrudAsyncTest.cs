using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Core;
using Overhaul.Data.Attributes;
using Overhaul.Interface;

namespace OverhaulTests.Core
{
    [TestClass]
    public sealed class CrudAsyncTest
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
        public async Task CreateTest()
        {
            // Arrange
            model = CreateCrudInstance();
            var document = new Document();

            // Act
            model.Create(document);

            // Assert
            var item = await model.ReadAsync<Document>();
            Assert.IsNotNull(item);
            Assert.IsTrue(document.DocumentId > 0);
        }

        [TestMethod]
        public async Task GetByIdTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select(i => new Document
                {
                    DocumentState = DocumentState.None
                })
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var doc = await model.GetByIdAsync<Document>(5);

            // Assert
            Assert.AreEqual(5, doc.DocumentId);
            Assert.AreEqual(DocumentState.None, doc.DocumentState);
        }
        [TestMethod]
        public async Task GetByIdColumnsTest()
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
            var doc = await model.GetByIdAsync<Document>(5,
                nameof(Document.Bool),
                nameof(Document.Guid));

            // Assert
            Assert.AreEqual(5, doc.DocumentId);
            Assert.AreEqual(true, doc.Bool);
            Assert.AreEqual(guid, doc.Guid);
        }

        [TestMethod]
        public async Task GetByTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 10)
                .Select((d, index) => new Document
                {
                    String = $"document{index}",
                    Int = index
                })
                .AsParallel()
                .ForAll(d => model.Create(d));

            // Act
            var document = await model.GetByAsync<Document>(nameof(Document.String), "document5");
            var doc2 = await model.GetByAsync<Document>(nameof(Document.Int), 9);

            // Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(doc2);
            Assert.AreEqual("document5", document.String);
            Assert.AreEqual(9, doc2.Int);
        }
        [TestMethod]
        public async Task GetByColumnTest()
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
            var document = await model.GetByAsync<Document>(nameof(Document.String),
                "document5", nameof(Document.String));

            var doc2 = await model.GetByAsync<Document>(nameof(Document.Int),
                9, nameof(Document.Int));

            // Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(doc2);
            Assert.AreEqual("document5", document.String);
            Assert.AreEqual(9, doc2.Int);
        }

        [TestMethod]
        public async Task GetCollectionTest()
        {
            // Arrange
            model = CreateCrudInstance();

            Enumerable.Range(0, 50)
                .Select(i => new Document())
                .AsParallel().ForAll(d => model.Create(d));

            // Act
            var collection = await model.GetCollectionAsync<Document>();

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(50, collection.Count());
        }
        [TestMethod]
        public async Task GetCollectionColumnTest()
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
            var collection = await model.GetCollectionAsync<Document>(nameof(Document.String));

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(50, collection.Count());
            Assert.AreEqual(50, collection.Count(i => i.String.Length == str.Length));
        }

        [TestMethod]
        public async Task GetCollectionWhereTest()
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
            var collection = await model.GetCollectionWhereAsync<Document>(nameof(Document.Bool), true);

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(10, collection.Count());
            Assert.IsTrue(collection.Count(i => i.Bool) == 10);
        }
        [TestMethod]
        public async Task GetCollectionWhereColumnTest()
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
            var collection = await model.GetCollectionWhereAsync<Document>(nameof(Document.Bool),
                true, nameof(Document.String), nameof(Document.Bool));

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(10, collection.Count());
            Assert.IsTrue(collection.Count(i => i.Bool) == 10);
            Assert.IsTrue(collection.Count(i => i.Int == 0) == 10);
            Assert.IsTrue(collection.Count(i => i.String == str) == 10);
        }

        [TestMethod()]
        public async Task UpdateTest()
        {
            // Arrange
            model = CreateCrudInstance();
            var document = new Document();

            model.Create(document);

            // Arrange
            document.Bool = true;

            // Act
            await model.UpdateAsync(document);

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
            public DocumentState DocumentState { get; set; }
        }

        public enum DocumentState : int
        {
            Null = 0,
            None
        }
    }
}
