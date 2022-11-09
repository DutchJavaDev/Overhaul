using Microsoft.VisualStudio.TestTools.UnitTesting;

using Overhaul.Core;
namespace Overhaul.Data.Attributes.Tests
{
    [TestClass()]
    public class PrecisionAttributeTests
    {
        [TestMethod()]
        public void StringPrecisionAttributeTest()
        {
            // Arrange
            var def = ModelTracker.CreateDefinitions(new[] { typeof(AttributeTests) })
                .First();

            // Assert
            Assert.AreEqual("BingoPlayer NVARCHAR(1024)",def.ColumnCollection);
        }

        [TestMethod()]
        public void DecimalPrecisionAttributeTest()
        {
            // Arrange`
            var def = ModelTracker.CreateDefinitions(new[] { typeof(AttributeTests2) })
                .First();

            // Assert
            Assert.AreEqual("BingoPlayer DECIMAL(5,3)", def.ColumnCollection);
        }


        [TestMethod()]
        public void CharPrecisionAttributeTest()
        {
            // Arrange`
            var def = ModelTracker.CreateDefinitions(new[] { typeof(AttributeTests3) })
                .First();

            // Assert
            Assert.AreEqual("BingoPlayer CHAR(1)", def.ColumnCollection);
        }

        public sealed class AttributeTests
        {
            [Precision(1024)]
            public string BingoPlayer { get; set; }
        }

        public sealed class AttributeTests2
        {
            [Precision(5.3)]
            public decimal BingoPlayer { get; set; }
        }

        // Im lazy
        public sealed class AttributeTests3
        {
            [Precision(1)]
            public char BingoPlayer { get; set; }
        }
    }
}