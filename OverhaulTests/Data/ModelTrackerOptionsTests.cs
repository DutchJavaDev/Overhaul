using Dapper.Contrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overhaul.Core;
using Overhaul.Interface;
using OverhaulTests;

namespace Overhaul.Data.Tests
{
    [TestClass]
    public class ModelTrackerOptionsTests
    {
        // Helpers
        private string ConnectionString { get; set; }

        // Model
        private ModelTrackerOptions model;
        Type[] Type1 = new[] { typeof(Class1) };
        Type[] Type1_2 = new[] { typeof(Class2) };


        [TestInitialize]
        public void Init()
        {
            ConnectionString = TestHelper.GetString("devString");
            ConnectionManager.SetConnectionString(ConnectionString);
            model = new()
            {
                DataLose = true // DANGERRRRRRRRRRRRRRRR
            };
        }


        [TestMethod]
        public void DataLossTest()
        {
            // Arrange 
            var tracker = CreateModelTracker(Type1);

            // Act
            tracker = CreateModelTracker(Type1_2);

            // Assert
            var crud = tracker.GetCrudInstance();

            try
            {
                // Should not be possible since
                // Class1String has been deleted
                crud.Create(new Class1
                {
                    Class1String = "sdfsdf",
                    Class1Int = 5465
                });
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains(nameof(Class1.Class1String)));
            }
        }

        private IModelTracker CreateModelTracker(IEnumerable<Type> types)
        {
            ModelTracker.DeleteTestTables(types, ConnectionString);

            IModelTracker tracker = new ModelTracker(ConnectionString, model);
            tracker.Track(types);
            return tracker;
        }
    }


    [Table("tblDestrcut")]
    sealed class Class1
    {
        public string Class1String { get; set; }
        public int Class1Int { get; set; }
    }


    [Table("tblDestrcut")]
    sealed class Class2
    {
        public int Class1Int { get; set; }
    }
}