using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Configuration.Tests
{
     [TestClass]
     public class ConfigurationTests
     {
          private readonly ILogger mFakeLogger = A.Dummy<ILogger>();
          private const string TestDatabasePath = @"Resources\DatabaseConfig.yaml";

          [TestMethod]
          public void GetDatabasePath_NoException()
          {
               DatabaseLocalConfigurtaion configuration = new DatabaseLocalConfigurtaion(mFakeLogger)
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };
               Assert.AreEqual("this_path_should_be_updated", configuration.DatabaseDirectoryPath);
          }

          [TestMethod]
          public void SetDatabasePath_Success()
          {
               DatabaseLocalConfigurtaion configuration = new DatabaseLocalConfigurtaion(mFakeLogger)
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };

               string newDatabasePathString = "new_database_path";
               
               configuration.SetDatabaseDirectoryPath(newDatabasePathString);
               Assert.AreEqual(configuration.DatabaseDirectoryPath, newDatabasePathString);
          }

          [TestMethod]
          public void GetNotesPath_NoException()
          {
               DatabaseLocalConfigurtaion configuration = new DatabaseLocalConfigurtaion(mFakeLogger)
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };
               Assert.AreEqual("this_notes_path_should_be_updated", configuration.NotesDirectoryPath);
          }

          [TestMethod]
          public void SetNotesPath_Success()
          {
               DatabaseLocalConfigurtaion configuration = new DatabaseLocalConfigurtaion(mFakeLogger)
               {
                    ConfigurationYamlFilePath = TestDatabasePath
               };

               string newNotesPathString = "new_notes_path";

               configuration.SetNotesDirectoryPath(newNotesPathString);
               Assert.AreEqual(configuration.NotesDirectoryPath, newNotesPathString);
          }
     }
}
