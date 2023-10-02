using Dbhaul.DataModels.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbhaul.Core.Tests
{
    [TestClass()]
    public class PSQLScriptGeneratorTests
    {
        [TestMethod()]
        public void CreateBuildScriptTest()
        {
            var builder = new PSQLScriptGenerator(new[] 
            { 
                typeof(PostModel), typeof(UserModel),
                typeof(ArticleModel), typeof(TagModel)
            });

            var script = builder.CreateBuildScript();

            Assert.IsNotNull(script);
        }
    }
}