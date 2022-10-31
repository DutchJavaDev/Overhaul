﻿using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Data;
using Overhaul.Interface;
using OverhaulTests;

namespace Overhaul.Core.Tests
{
    [TestClass()]
    public sealed class SqlGeneratorTests
    {
        private ISqlGenerator model { get; set; }

        [TestInitialize]
        public void Init()
        {
            model = new SqlGenerator(TestHelper.GetString("devString"));
        }

        // exclude until fixed
        //[TestMethod]
        public void BGetcollectionTest()
        {
            // Act
            var result = model.GetCollection();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ACreateTableTest()
        {
            // Arrange
            var def = CreateDef();

            // Act
            var result = model.CreateTable(def);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CDeleteTableTest()
        {
            // Arrange
            var def = CreateDef();

            // Act
            var result = model.DeleteTable(def);

            // Assert
            Assert.IsTrue(result);
        }

        private static TableDef CreateDef()
        {
            var type = typeof(TableClass);
            var def = ModelTracker.CreateDefinitions(new[] { type })
                .First();
            return def;
        }

        [Table("tblTblaClass")]
        public sealed class TableClass
        {
            public string Name { get; set; } = string.Empty;
            public int Number { get; set; }
            public Guid Description { get; set; }
        }
    }
}