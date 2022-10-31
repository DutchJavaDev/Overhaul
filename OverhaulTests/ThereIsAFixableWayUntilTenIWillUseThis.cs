using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OverhaulTests
{
    [TestClass]
    public class ThereIsAFixableWayUntilTenIWillUseThis
    {
        [TestMethod]
        public void GetConnectionStringTest()
        {
            // Act
            var connectionString = TestHelper.GetString("secrete");

            // Assert
            Assert.AreEqual("found me!",connectionString);
        }
    }
}
