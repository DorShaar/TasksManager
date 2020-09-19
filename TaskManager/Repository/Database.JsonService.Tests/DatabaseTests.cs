using Database.Configuration;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<DatabaseLocalConfigurtaion> mConfiguration =
            A.Dummy<IOptions<DatabaseLocalConfigurtaion>>();
        private readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
        private readonly ITasksGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();

        [TestMethod]
        public void GetAll_Returns3Entities()
        {
            Database database = CreateTestsDatabase();
            Assert.AreEqual(3, database.GetAll().Count());
        }

        [TestMethod]
        public void GetEntity_ExistingGroupNames_Found()
        {
            Database database = CreateTestsDatabase();
            Assert.IsNotNull(database.GetEntity("A"));
            Assert.IsNotNull(database.GetEntity("B"));
            Assert.IsNotNull(database.GetEntity("C"));
        }

        [TestMethod]
        public void GetEntity_NotExistingGroupNames_NotFound()
        {
            Database database = CreateTestsDatabase();
            Assert.IsNull(database.GetEntity("a"));
            Assert.IsNull(database.GetEntity("X"));
        }

        [TestMethod]
        public void Insert_AlreadyExistingName_NoChange()
        {
            Database database = CreateTestsDatabase();
            int sizeBefore = database.GetAll().Count();
            database.Insert(mTaskGroupBuilder.Create("A", mLogger));
            int sizeAfter = database.GetAll().Count();
            Assert.AreEqual(sizeBefore, sizeAfter);
        }

        [TestMethod]
        public void Update_EntityUpdated()
        {
            Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

            taskGroup.AddTask(new WorkTask("some group", "todo A1", mLogger));
            taskGroup.AddTask(new WorkTask("some group", "todo A2", mLogger));
            database.Update(taskGroup);
            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingEntity_EntityUpdated()
        {
            Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

            taskGroup.AddTask(new WorkTask("some group", "todo A1", mLogger));
            taskGroup.AddTask(new WorkTask("some group", "todo A2", mLogger));
            database.AddOrUpdate(taskGroup);
            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
        }

        [TestMethod]
        public void AddOrUpdate_NewEntity_EntityAdded()
        {
            Database database = CreateTestsDatabase();

            const string newTaskGroupName = "X";
            database.Insert(mTaskGroupBuilder.Create(newTaskGroupName, mLogger));
            ITasksGroup taskGroup = database.GetEntity(newTaskGroupName);
            database.AddOrUpdate(taskGroup);
            Assert.AreEqual(database.GetAll().Count(), 4);
        }

        [TestMethod]
        public void Remove_EntityRemoved()
        {
            Database database = CreateTestsDatabase();
            int sizeBeforeDelete = database.GetAll().Count();

            ITasksGroup taskGroupToRemove = database.GetEntity("A");
            database.Remove(taskGroupToRemove);
            int sizeAfterDelete = database.GetAll().Count();

            Assert.AreEqual(sizeBeforeDelete, sizeAfterDelete + 1);
        }

        [TestMethod]
        public void RemoveByName_EntityRemoved()
        {
            Database database = CreateTestsDatabase();
            int sizeBeforeDelete = database.GetAll().Count();

            ITasksGroup entity = database.GetEntity("A");
            database.Remove(entity);
            int sizeAfterDelete = database.GetAll().Count();

            Assert.AreEqual(sizeBeforeDelete, sizeAfterDelete + 1);
        }

        private Database CreateTestsDatabase()
        {
            Database database = new Database(mConfiguration, mSerializer, mLogger);
            database.Insert(mTaskGroupBuilder.Create("A", mLogger));
            database.Insert(mTaskGroupBuilder.Create("B", mLogger));
            database.Insert(mTaskGroupBuilder.Create("C", mLogger));

            return database;
        }
    }
}