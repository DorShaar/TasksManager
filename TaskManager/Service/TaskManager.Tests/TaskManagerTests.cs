using Database.Contracts;
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
using System.Collections.Generic;

namespace TaskManager.Integration.Tests
{
    [TestClass]
    public class TaskManagerTests
    {
        private static readonly ILogger mLogger = A.Dummy<ILogger>();
        private static readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
        private static readonly ITasksGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();
        private static readonly INoteBuilder mNoteBuilder = new NoteBuilder();
        private static readonly INotesSubjectBuilder mNotesSubjectBuilder = new NotesSubjectBuilder();
        private static IOptions<DatabaseLocalConfigurtaion> mFakeConfiguration;
        private static ILocalRepository<ITasksGroup> mDatabase;
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

            mDatabase = new Database.Database(mFakeConfiguration, mSerializer, mLogger);
            mTaskManager = new TaskManager(mDatabase, mTaskGroupBuilder, mNoteBuilder, mNotesSubjectBuilder, mLogger);
        }

        [TestMethod]
        public void GetAllTasksByGroup_3Tasks_3TasksReturned()
        {
            ITasksGroup taskGroup = mTaskGroupBuilder.Create("A", mLogger);
            IWorkTask task1 = mTaskManager.CreateNewTask(taskGroup, "1");
            IWorkTask task2 = mTaskManager.CreateNewTask(taskGroup, "2");
            IWorkTask task3 = mTaskManager.CreateNewTask(taskGroup, "3");

            Assert.AreEqual(mTaskManager.GetAllTasks((ITasksGroup group) => group.ID == taskGroup.ID).Count(), 3);
        }

        [TestMethod]
        public void GetAllTasksByTask_ClosedTasks_3TasksReturned()
        {
            ITasksGroup taskGroupA = mTaskGroupBuilder.Create("A", mLogger);
            IWorkTask task1 = mTaskManager.CreateNewTask(taskGroupA, "A1");
            IWorkTask task2 = mTaskManager.CreateNewTask(taskGroupA, "A2");
            mTaskManager.CloseTask(task1.ID, string.Empty);

            ITasksGroup taskGroupB = mTaskGroupBuilder.Create("B", mLogger);
            IWorkTask task3 = mTaskManager.CreateNewTask(taskGroupB, "B1");
            IWorkTask task4 = mTaskManager.CreateNewTask(taskGroupB, "B2");
            mTaskManager.CloseTask(task4.ID, string.Empty);

            ITasksGroup taskGroupC = mTaskGroupBuilder.Create("C", mLogger);
            IWorkTask task5 = mTaskManager.CreateNewTask(taskGroupC, "C1");
            mTaskManager.CloseTask(task5.ID, string.Empty);

            Assert.AreEqual(mTaskManager.GetAllTasks((IWorkTask task) => task.IsFinished == true).Count(), 3);
        }

        [TestMethod]
        public void Ctor_TasksManagerHasFreeTasksGroup()
        {
            Assert.IsNotNull(mTaskManager.GetAllTasks(taskGrop => taskGrop.GroupName == TaskManager.FreeTaskGroupName));
        }

        [TestMethod]
        public void CreateNewTask_AddNewTaskToGroup_Success()
        {
            ITasksGroup taskGroup = mTaskGroupBuilder.Create("A", mLogger);
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
        public void RemoveTaskGroup_ExistingGroup_Success()
        {
            string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            Assert.AreEqual(mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName).Count(), 0);

            mTaskManager.RemoveTaskGroup(taskGroupName, false);
            IEnumerable<IWorkTask> workTasks = mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName);
            Assert.IsFalse(workTasks.Any());
        }

        [TestMethod]
        public void MoveTask_TaskInDestinationGroup_Fail()
        {
            string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            ITasksGroup taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.Name == taskGroupName).First();

            string taskDescription = "new task";
            mTaskManager.CreateNewTask(taskGroup, taskDescription);
            Assert.AreEqual(1, taskGroup.Size);

            IWorkTask taskToMove = mTaskManager.GetAllTasks(task => task.Description == taskDescription).First();
            mTaskManager.MoveTaskToGroup(taskToMove.ID, taskGroupName);
            Assert.AreEqual(1, taskGroup.Size);
        }

        [TestMethod]
        public void MoveTask_Success()
        {
            string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            ITasksGroup taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.Name == taskGroupName).First();
            Assert.AreEqual(0, taskGroup.Size);

            // Create new task in free task group.
            string taskDescription = "new task";
            mTaskManager.CreateNewTask(taskDescription);
            Assert.AreEqual(1, mTaskManager.FreeTasksGroup.Size);

            IWorkTask taskToMove = mTaskManager.GetAllTasks(task => task.Description == taskDescription).First();
            mTaskManager.MoveTaskToGroup(taskToMove.ID, taskGroupName);

            Assert.AreEqual(1, taskGroup.Size);
            Assert.AreEqual(0, mTaskManager.FreeTasksGroup.Size);
        }
    }
}