using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Core;
using OverhaulTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overhaul.Core.Tests
{
    [TestClass]
    public class ConnectionManagerTests
    {
        [TestMethod]
        public void GetSqlConnectionTest()
        {
            var connectionString = TestHelper.GetString("devString");
            ConnectionManager.SetConnectionString(connectionString);

            using var conn = ConnectionManager.GetSqlConnection();
            Assert.IsTrue(conn.State == System.Data.ConnectionState.Open);
        }
    }
}