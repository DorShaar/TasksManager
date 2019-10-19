using Database.Contracts;
using Database;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TaskData;
using TaskData.Contracts;
using ObjectSerializer.Contracts;
using Microsoft.Extensions.Options;
using Database.Configuration;
using System.IO;

namespace TaskManager.Integration.Tests
{
    [TestClass]
    public class TaskManagerTests
    {
        private static readonly ILogger mLogger = A.Dummy<ILogger>();
        private static readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
        private static readonly ITaskGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();
        private static readonly INoteBuilder mNoteBuilder = new NoteBuilder();
        private static readonly INotesSubjectBuilder mNotesSubjectBuilder = new NotesSubjectBuilder();
        private static IOptions<DatabaseLocalConfigurtaion> mFakeConfiguration;
        private static ILocalRepository<ITaskGroup> mDatabase;
        private static TaskManager mTaskManager;

        [TestInitialize]
        public void Startup()
        {
            DatabaseLocalConfigurtaion fakeLocalConfigurations = new DatabaseLocalConfigurtaion()
            {
                NotesTasksDirectoryPath = Directory.GetCurrentDirectory(),
                DatabaseDirectoryPath = Directory.GetCurrentDirectory(),
                NotesDirectoryPath = Directory.GetCurrentDirectory()
            };
            mFakeConfiguration = Options.Create(fakeLocalConfigurations);

            mDatabase = new Database<ITaskGroup>(mFakeConfiguration, mSerializer, mLogger);
            mTaskManager = new TaskManager(mDatabase, mTaskGroupBuilder, mNoteBuilder, mNotesSubjectBuilder, mLogger);
        }

        [TestMethod]
        public void GetAllTasksByGroup_3Tasks_3TasksReturned()
        {
            ITaskGroup taskGroup = mTaskGroupBuilder.Create("A", mLogger);
            ITask task1 = mTaskManager.CreateNewTask(taskGroup, "1");
            ITask task2 = mTaskManager.CreateNewTask(taskGroup, "2");
            ITask task3 = mTaskManager.CreateNewTask(taskGroup, "3");

            Assert.AreEqual(mTaskManager.GetAllTasks((ITaskGroup group) => group.ID == taskGroup.ID).Count(), 3);
        }

        [TestMethod]
        public void GetAllTasksByTask_ClosedTasks_3TasksReturned()
        {
            ITaskGroup taskGroupA = mTaskGroupBuilder.Create("A", mLogger);
            ITask task1 = mTaskManager.CreateNewTask(taskGroupA, "A1");
            ITask task2 = mTaskManager.CreateNewTask(taskGroupA, "A2");
            mTaskManager.CloseTask(task1.ID, string.Empty);

            ITaskGroup taskGroupB = mTaskGroupBuilder.Create("B", mLogger);
            ITask task3 = mTaskManager.CreateNewTask(taskGroupB, "B1");
            ITask task4 = mTaskManager.CreateNewTask(taskGroupB, "B2");
            mTaskManager.CloseTask(task4.ID, string.Empty);

            ITaskGroup taskGroupC = mTaskGroupBuilder.Create("C", mLogger);
            ITask task5 = mTaskManager.CreateNewTask(taskGroupC, "C1");
            mTaskManager.CloseTask(task5.ID, string.Empty);

            Assert.AreEqual(mTaskManager.GetAllTasks((ITask task) => task.IsFinished == true).Count(), 3);
        }

        [TestMethod]
        public void Ctor_TasksManagerHasFreeTasksGroup()
        {
            Assert.IsNotNull(mTaskManager.GetAllTasks(taskGrop => taskGrop.GroupName == TaskManager.FreeTaskGroupName));
        }

        [TestMethod]
        public void CreateNewTask_AddNewTaskToGroup_Success()
        {
            ITaskGroup taskGroup = mTaskGroupBuilder.Create("A", mLogger);
            mTaskManager.CreateNewTask(taskGroup, "New Task Group");
            Assert.AreEqual(mTaskManager.GetAllTasksGroups().Count(), 2);
        }

        [TestMethod]
        public void CreateNewTask_AddNewTaskToFreeGroup_Success()
        {
            mTaskManager.CreateNewTask("New Task Group");
            Assert.AreEqual(1, mTaskManager.GetAllTasksGroups().Count());
        }

        [TestMethod]
        public void RemoveTask_ExistingGroup_Success()
        {
            string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            Assert.AreEqual(mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName).Count(), 0);

            mTaskManager.RemoveTaskGroup(taskGroupName, false);
            Assert.IsNull(mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName));
        }
    }
}