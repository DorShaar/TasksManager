using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Configuration.Tests
{
     [TestClass]
     public class ConfigurationTests
     {
          private const string TestDatabasePath = @"Resources\DatabaseConfig.yaml";

          [TestMethod]
          public void GetDatabasePath_NoException()
          {
               Configuration configuration = new Configuration
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };
               Assert.AreEqual("this_path_should_be_updated", configuration.DatabaseDirectoryPath);
          }

          [TestMethod]
          public void SetDatabasePath_Success()
          {
               Configuration configuration = new Configuration()
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };

               string newDatabasePathString = "new_database_path";
               
               configuration.SetDatabaseDirectoryPath(newDatabasePathString);
               Assert.AreEqual(configuration.DatabaseDirectoryPath, newDatabasePathString);
          }
     }
}
