using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Core;
using Overhaul.Interface;
using OverhaulTests;
using System;
namespace Overhaul.Core.Tests
{
    [TestClass()]
    public sealed class SqlGeneratorTests
    {
        private ISqlGenerator model { get; set; }

        [TestInitialize]
        public void Init()
        {
            model = new SqlGenerator(TestHelper.GetConnectionString());
        }

        [TestMethod()]
        public void SqlGeneratorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TableExistsTest()
        {
            Assert.Fail();
        }
    }
}