using Database.Contracts;
using Database;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TaskData;
using TaskData.Contracts;
using ObjectSerializer.Contracts;

namespace TaskManager.Integration.Tests
{
    [TestClass]
    public class TaskManagerTests
    {
        private static readonly ILogger mLogger = A.Dummy<ILogger>();
        private static readonly IConfiguration mConfiguration = A.Dummy<IConfiguration>();
        private static readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
        private static readonly ITaskGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();
        private static IRepository<ITaskGroup> mDatabase = new Database<ITaskGroup>(mConfiguration, mSerializer, mLogger);
        private static TaskManager mTaskManager = new TaskManager(mDatabase, mTaskGroupBuilder, mLogger);

        [TestInitialize]
        public void Startup()
        {
            mDatabase = new Database<ITaskGroup>(mConfiguration, mSerializer, mLogger);
            mTaskManager = new TaskManager(mDatabase, mTaskGroupBuilder, mLogger);
        }

        [TestMethod]
        public void GetAllTasksByGroup_3Tasks_3TasksReturned()
        {
            string taskGroupName = "A";

            ITaskGroup taskGroup = mTaskGroupBuilder.Create(taskGroupName, mLogger);
            ITask task1 = mTaskManager.CreateNewTask(taskGroupName, "1");
            ITask task2 = mTaskManager.CreateNewTask(taskGroupName, "2");
            ITask task3 = mTaskManager.CreateNewTask(taskGroupName, "3");

            Assert.AreEqual(mTaskManager.GetAllTasks((ITaskGroup group) => group.ID == taskGroup.ID).Count(), 3);
        }

        [TestMethod]
        public void GetAllTasksByTask_ClosedTasks_3TasksReturned()
        {
            string taskGroupAName = "A";
            string taskGroupBName = "B";
            string taskGroupCName = "C";

            ITaskGroup taskGroupA = mTaskGroupBuilder.Create(taskGroupAName, mLogger);
            ITask task1 = mTaskManager.CreateNewTask(taskGroupAName, "A1");
            ITask task2 = mTaskManager.CreateNewTask(taskGroupAName, "A2");
            mTaskManager.CloseTask(task1.ID);

            ITaskGroup taskGroupB = mTaskGroupBuilder.Create(taskGroupBName, mLogger);
            ITask task3 = mTaskManager.CreateNewTask(taskGroupBName, "B1");
            ITask task4 = mTaskManager.CreateNewTask(taskGroupBName, "B2");
            mTaskManager.CloseTask(task4.ID);

            ITaskGroup taskGroupC = mTaskGroupBuilder.Create(taskGroupCName, mLogger);
            ITask task5 = mTaskManager.CreateNewTask(taskGroupCName, "C1");
            mTaskManager.CloseTask(task5.ID);

            Assert.AreEqual(mTaskManager.GetAllTasks(group => group.IsFinished == true).Count(), 3);
            Assert.AreEqual(mTaskManager.GetAllTasks(task => task.IsFinished == true).Count(), 3);
        }

        [TestMethod]
        public void Ctor_TasksManagerHasFreeTasksGroup()
        {
            Assert.IsNotNull(mTaskManager.GetAllTasks(taskGrop => taskGrop.GroupName == TaskManager.FreeTaskGroupName));
        }

        [TestMethod]
        public void CreateNewTask_AddNewTaskToGroup_Success()
        {
            string taskGroupName = "A";
            mTaskGroupBuilder.Create(taskGroupName, mLogger);
            mTaskManager.CreateNewTask(taskGroupName, "New Task Group");
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