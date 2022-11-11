using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Interface;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class CrudTests
    {
        // Model
        private ICrud model;

        // Helpers
        private readonly Type[] Types = new[] { typeof(Document) }; 

        [TestInitialize]
        public void Init()
        {
        }


        [TestMethod()]
        public void CrudTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
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
            public double Double { get; set; }
            public Guid Guid { get; set; }
            public short Short { get; set; }
            public byte Byte { get; set; }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}