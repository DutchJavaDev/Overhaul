using Microsoft.VisualStudio.TestTools.UnitTesting;

using Overhaul.Core;
using Overhaul.Data.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overhaul.Data.Attributes.Tests
{
    [TestClass()]
    public class StringPrecisionAttributeTests
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

        public sealed class AttributeTests
        {
            [Precision(1024)]
            public string BingoPlayer { get; set; }
        }
    }
}