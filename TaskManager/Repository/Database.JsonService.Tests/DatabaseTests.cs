using Database.Contracts;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectSerializer.Contracts;
using System.Linq;
using TaskData;
using TaskData.Contracts;

namespace Database.Tests
{
     [TestClass]
     public class DatabaseTests
     {
          private readonly ILogger mLogger = A.Dummy<ILogger>();
          private readonly IConfiguration mConfiguration = A.Dummy<IConfiguration>();
          private readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
          private readonly ITaskGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();

          [TestMethod]
          public void GetAll_Returns3Entities()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               Assert.AreEqual(3, database.GetAll().Count());
          }

          [TestMethod]
          public void GetByName_ExistingGroupNames_Found()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               Assert.IsNotNull(database.GetByName("A"));
               Assert.IsNotNull(database.GetByName("B"));
               Assert.IsNotNull(database.GetByName("C"));
          }

          [TestMethod]
          public void GetByName_NotExistingGroupNames_NotFound()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               Assert.IsNull(database.GetByName("a"));
               Assert.IsNull(database.GetByName("X"));
          }

          [TestMethod]
          public void Insert_AlreadyExistingName_NoChange()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               int sizeBefore = database.GetAll().Count();
               database.Insert(mTaskGroupBuilder.Create("A", mLogger));
               int sizeAfter = database.GetAll().Count();
               Assert.AreEqual(sizeBefore, sizeAfter);
          }

          [TestMethod]
          public void Update_EntityUpdated()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               ITaskGroup taskGroup = database.GetByName("A");
               Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

               taskGroup.AddTask(new Task("todo A1", mLogger));
               taskGroup.AddTask(new Task("todo A2", mLogger));
               database.Update(taskGroup);
               ITaskGroup updatedTaskGroup = database.GetByName("A");
               Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
          }

          [TestMethod]
          public void AddOrUpdate_ExistingEntity_EntityUpdated()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               ITaskGroup taskGroup = database.GetByName("A");
               Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

               taskGroup.AddTask(new Task("todo A1", mLogger));
               taskGroup.AddTask(new Task("todo A2", mLogger));
               database.AddOrUpdate(taskGroup);
               ITaskGroup updatedTaskGroup = database.GetByName("A");
               Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
          }

          [TestMethod]
          public void AddOrUpdate_NewEntity_EntityAdded()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();

               string newTaskGroupName = "X";
               database.Insert(mTaskGroupBuilder.Create(newTaskGroupName, mLogger));
               ITaskGroup taskGroup = database.GetByName(newTaskGroupName);
               database.AddOrUpdate(taskGroup);
               Assert.AreEqual(database.GetAll().Count(), 4);
          }

          [TestMethod]
          public void Remove_EntityRemoved()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               int sizeBeforeDelete = database.GetAll().Count();

               ITaskGroup taskGroupToRemove = database.GetByName("A");
               database.Remove(taskGroupToRemove);
               int sizeAfterDelete = database.GetAll().Count();

               Assert.AreEqual(sizeBeforeDelete, sizeAfterDelete + 1);
          }

          [TestMethod]
          public void RemoveByName_EntityRemoved()
          {
               Database<ITaskGroup> database = CreateTestsDatabase();
               int sizeBeforeDelete = database.GetAll().Count();

               database.RemoveByName("A");
               int sizeAfterDelete = database.GetAll().Count();

               Assert.AreEqual(sizeBeforeDelete, sizeAfterDelete + 1);
          }

          private Database<ITaskGroup> CreateTestsDatabase()
          {
               Database<ITaskGroup> database = new Database<ITaskGroup>(mConfiguration, mSerializer, mLogger);
               database.Insert(mTaskGroupBuilder.Create("A", mLogger));
               database.Insert(mTaskGroupBuilder.Create("B", mLogger));
               database.Insert(mTaskGroupBuilder.Create("C", mLogger));

               return database;
          }
     }
}