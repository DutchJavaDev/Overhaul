using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Overhaul.Common.Tests
{
    [TestClass()]
    public class SupportedTests
    {

        private Type _type = typeof(SqlTypesClass);

        [TestMethod()]
        public void GetPropertiesForTypeTest()
        {
            // Arrange

            // Act
            var types = Supported.GetPropertiesForType(_type);

            // Assert
            Assert.IsTrue(types.Any(i => i.Name.Equals("A")));
            Assert.IsTrue(types.Any(i => i.Name.Equals("B")));
        }

        [TestMethod()]
        public void ConvertPropertiesToTypesStringTest()
        {
            // Arrange
            var types = Supported.GetPropertiesForType(_type);

            // Act
            var strQuery = Supported.ConvertPropertiesToTypesString(types);

            // Assert

            Assert.AreEqual("A INT,B NVARCHAR(255)", strQuery);
        }


        public sealed class SqlTypesClass
        {
            public int A { get; set; }

            public string B { get; set; } = string.Empty;
        }
    }
}