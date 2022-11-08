using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Overhaul.Core;
using Overhaul.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public class CrudTests
    {
        // Model
        private ICrud model;

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


        public sealed class Document
        {
            [Key]
            public int DocumentId { get; set; }
            public int DocumentNumber { get; set; }
            public string DocumentTitle { get; set; } = string.Empty;
            public bool Valid { get; set; }
        }
    }
}