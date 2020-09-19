using Database.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System.Linq;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Xunit;

namespace Database.Tests
{
    public class DatabaseTests
    {
        private readonly IOptions<DatabaseLocalConfigurtaion> mConfiguration = A.Dummy<IOptions<DatabaseLocalConfigurtaion>>();

        [Fact]
        public void GetAll_Returns3Entities()
        {
            Databases.Database database = CreateTestsDatabase();
            Assert.Equal(3, database.GetAll().Count());
        }

        [Fact]
        public void GetEntity_ExistingGroupNames_Found()
        {
            Databases.Database database = CreateTestsDatabase();
            Assert.NotNull(database.GetEntity("A"));
            Assert.NotNull(database.GetEntity("B"));
            Assert.NotNull(database.GetEntity("C"));
        }

        [Fact]
        public void GetEntity_NotExistingGroupNames_NotFound()
        {
            Databases.Database database = CreateTestsDatabase();
            Assert.Null(database.GetEntity("a"));
            Assert.Null(database.GetEntity("X"));
        }

        [Fact]
        public void Insert_AlreadyExistingName_NoChange()
        {
            Databases.Database database = CreateTestsDatabase();
            int sizeBefore = database.GetAll().Count();
            database.Insert(CreateFakeGroup("1001", "A"));
            int sizeAfter = database.GetAll().Count();
            Assert.Equal(sizeBefore, sizeAfter);
        }

        [Fact]
        public void Update_EntityUpdated()
        {
            Databases.Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.Empty(taskGroup.GetAllTasks());

            IWorkTask workTask1 = CreateFakeTask(taskGroup.Name, "todo A1");
            IWorkTask workTask2 = CreateFakeTask(taskGroup.Name, "todo A2");
            taskGroup.AddTask(workTask1);
            taskGroup.AddTask(workTask2);
            database.Update(taskGroup);
            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.Equal(2, updatedTaskGroup.GetAllTasks().Count());
        }

        [Fact]
        public void AddOrUpdate_ExistingEntity_EntityUpdated()
        {
            Databases.Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.Empty(taskGroup.GetAllTasks());

            IWorkTask workTask1 = CreateFakeTask(taskGroup.Name, "todo A1");
            IWorkTask workTask2 = CreateFakeTask(taskGroup.Name, "todo A2");
            taskGroup.AddTask(workTask1);
            taskGroup.AddTask(workTask2);
            database.AddOrUpdate(taskGroup);

            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.Equal(2, updatedTaskGroup.GetAllTasks().Count());
        }

        [Fact]
        public void AddOrUpdate_NewEntity_EntityAdded()
        {
            Databases.Database database = CreateTestsDatabase();

            const string newTaskGroupName = "X";
            database.Insert(A.Dummy<ITasksGroup>());
            ITasksGroup taskGroup = database.GetEntity(newTaskGroupName);
            database.AddOrUpdate(taskGroup);
            Assert.Equal(4, database.GetAll().Count());
        }

        [Fact]
        public void Remove_EntityRemoved()
        {
            Databases.Database database = CreateTestsDatabase();
            int sizeBeforeDelete = database.GetAll().Count();

            ITasksGroup taskGroupToRemove = database.GetEntity("A");
            database.Remove(taskGroupToRemove);
            int sizeAfterDelete = database.GetAll().Count();

            Assert.Equal(sizeBeforeDelete, sizeAfterDelete + 1);
        }

        [Fact]
        public void RemoveByName_EntityRemoved()
        {
            Databases.Database database = CreateTestsDatabase();
            int sizeBeforeDelete = database.GetAll().Count();

            ITasksGroup entity = database.GetEntity("A");
            database.Remove(entity);
            int sizeAfterDelete = database.GetAll().Count();

            Assert.Equal(sizeBeforeDelete, sizeAfterDelete + 1);
        }

        private Databases.Database CreateTestsDatabase()
        {
            ITasksGroup tasksGroup1 = CreateFakeGroup("1001", "A");
            ITasksGroup tasksGroup2 = CreateFakeGroup("1002", "B");
            ITasksGroup tasksGroup3 = CreateFakeGroup("1003", "C");

            Databases.Database database = new Databases.Database(
                mConfiguration, A.Dummy<IObjectSerializer>(), A.Dummy<IIDProducer>(), NullLogger<Databases.Database>.Instance);
            database.Insert(tasksGroup1);
            database.Insert(tasksGroup2);
            database.Insert(tasksGroup3);

            return database;
        }

        private ITasksGroup CreateFakeGroup(string id, string name)
        {
            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.ID).Returns(id);
            A.CallTo(() => tasksGroup.Name).Returns(name);

            return tasksGroup;
        }

        private IWorkTask CreateFakeTask(string id, string groupName)
        {
            IWorkTask workTask = A.Fake<IWorkTask>();
            A.CallTo(() => workTask.ID).Returns(id);
            A.CallTo(() => workTask.GroupName).Returns(groupName);

            return workTask;
        }
    }
}