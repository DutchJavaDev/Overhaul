using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Overhaul.Common.Tests
{
    [TestClass()]
    public class SupportedTests
    {
        private readonly Type _type = typeof(SupportedTypesClass);

        [TestMethod]
        public void ValidProperyTest()
        {
            // Arrange
            var properties = _type.GetProperties();

            // Assert
            foreach (var prop in properties)
            {
                Assert.IsTrue(Supported.ValidProperty(prop));
            }
        }

        [TestMethod()]
        public void GetPropertiesForTypeTest()
        {
            // Act
            var types = Supported.GetPropertiesForType(_type);

            // Assert
            Assert.IsTrue(types.Any(i => i.Name.Equals("Int")));
            Assert.IsTrue(types.Any(i => i.Name.Equals("String")));
            // when im bored write the rest :)
        }

        [TestMethod()]
        public void ConvertPropertiesToTypesStringTest()
        {
            // Arrange
            var types = Supported.GetPropertiesForType(_type);

            // Act
            var strQuery = Supported.ConvertPropertiesToTypesString(types, out int count);

            // Assert
            var expectedSql = "Int INT,String NVARCHAR(255),"+
                                 "Float FLOAT,Decimal DECIMAL,"+
                                 "Char CHAR(2),Double FLOAT,"+
                                 "Guid UNIQUEIDENTIFIER,"+
                                 "Short SMALLINT,Byte TINYINT,"+ 
                                 "Bool BIT,DateTime DATETIME";

            Assert.AreEqual(expectedSql, strQuery);
            Assert.AreEqual(11, count);
        }
        public sealed class SupportedTypesClass
        {
            public int Int { get; set; }
            public string String { get; set; } = string.Empty;
            public float Float { get; set; }
            public decimal Decimal { get; set; }
            public char Char { get; set; }
            public double Double { get; set; }
            public Guid Guid { get; set; }
            public short Short { get; set; }
            public byte Byte { get; set;  }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}