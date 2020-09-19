using Database.Configuration;
using Databases;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.IDsProducer;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Xunit;

namespace TaskManager.Integration.Tests
{
    public class TaskManagerIntegrationTests
    {
        private static readonly IObjectSerializer mSerializer = A.Dummy<IObjectSerializer>();
        private static readonly INoteFactory mNoteFactory = new NoteFactory();
        private static IOptions<DatabaseLocalConfigurtaion> mFakeConfiguration;
        private static ILocalRepository<ITasksGroup> mDatabase;
        private static TaskManagers.TaskManager mTaskManager;

        private readonly ITasksGroupFactory mTaskGroupFactory;

        public TaskManagerIntegrationTests()
        {
            DatabaseLocalConfigurtaion fakeLocalConfigurations = new DatabaseLocalConfigurtaion()
            {
                NotesTasksDirectoryPath = Directory.GetCurrentDirectory(),
                DatabaseDirectoryPath = Directory.GetCurrentDirectory(),
                NotesDirectoryPath = Directory.GetCurrentDirectory()
            };
            mFakeConfiguration = Options.Create(fakeLocalConfigurations);

            IDProducer idProducer = new IDProducer();

            WorkTaskFactory workTaskFactory = new WorkTaskFactory(idProducer, NullLogger<WorkTaskFactory>.Instance);

            mTaskGroupFactory = new TaskGroupFactory(idProducer, workTaskFactory, NullLogger<TaskGroupFactory>.Instance);

            mDatabase = new Databases.Database(
                mFakeConfiguration, mSerializer, idProducer, NullLogger<Databases.Database>.Instance);

            mTaskManager = new TaskManagers.TaskManager(
                mDatabase, mTaskGroupFactory, mNoteFactory, NullLogger<TaskManagers.TaskManager>.Instance);
        }

        [Fact]
        public void GetAllTasksByGroup_3Tasks_3TasksReturned()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.Create("A");
            IWorkTask task1 = mTaskManager.CreateNewTask(taskGroup, "1");
            IWorkTask task2 = mTaskManager.CreateNewTask(taskGroup, "2");
            IWorkTask task3 = mTaskManager.CreateNewTask(taskGroup, "3");

            Assert.Equal(3, mTaskManager.GetAllTasks((ITasksGroup group) => group.ID == taskGroup.ID).Count());
        }

        [Fact]
        public void GetAllTasksByTask_ClosedTasks_3TasksReturned()
        {
            ITasksGroup taskGroupA = mTaskGroupFactory.Create("A");
            IWorkTask task1 = mTaskManager.CreateNewTask(taskGroupA, "A1");
            IWorkTask task2 = mTaskManager.CreateNewTask(taskGroupA, "A2");
            mTaskManager.CloseTask(task1.ID, string.Empty);

            ITasksGroup taskGroupB = mTaskGroupFactory.Create("B");
            IWorkTask task3 = mTaskManager.CreateNewTask(taskGroupB, "B1");
            IWorkTask task4 = mTaskManager.CreateNewTask(taskGroupB, "B2");
            mTaskManager.CloseTask(task4.ID, string.Empty);

            ITasksGroup taskGroupC = mTaskGroupFactory.Create("C");
            IWorkTask task5 = mTaskManager.CreateNewTask(taskGroupC, "C1");
            mTaskManager.CloseTask(task5.ID, string.Empty);

            Assert.Equal(3, mTaskManager.GetAllTasks((IWorkTask task) => task.IsFinished).Count());
        }

        [Fact]
        public void Ctor_TasksManagerHasFreeTasksGroup()
        {
            Assert.NotNull(mTaskManager.GetAllTasks(taskGrop => taskGrop.GroupName == TaskManagers.TaskManager.FreeTaskGroupName));
        }

        [Fact]
        public void CreateNewTask_AddNewTaskToGroup_Success()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.Create("A");
            mTaskManager.CreateNewTask(taskGroup, "New Task Group");
            Assert.Equal(2, mTaskManager.GetAllTasksGroups().Count());
        }

        [Fact]
        public void CreateNewTask_AddNewTaskToFreeGroup_Success()
        {
            mTaskManager.CreateNewTask("New Task Group");
            Assert.Single(mTaskManager.GetAllTasksGroups());
        }

        [Fact]
        public void RemoveTaskGroup_ExistingGroup_Success()
        {
            const string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            Assert.Empty(mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName));

            mTaskManager.RemoveTaskGroup(taskGroupName, false);
            IEnumerable<IWorkTask> workTasks = mTaskManager.GetAllTasks(taskGroup => taskGroup.GroupName == taskGroupName);
            Assert.False(workTasks.Any());
        }

        [Fact]
        public void MoveTask_TaskInDestinationGroup_Fail()
        {
            const string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            ITasksGroup taskGroup = mTaskManager.GetAllTasksGroups().First(group => group.Name == taskGroupName);

            const string taskDescription = "new task";
            mTaskManager.CreateNewTask(taskGroup, taskDescription);
            Assert.Equal(1, taskGroup.Size);

            IWorkTask taskToMove = mTaskManager.GetAllTasks(task => task.Description == taskDescription).First();
            mTaskManager.MoveTaskToGroup(taskToMove.ID, taskGroupName);
            Assert.Equal(1, taskGroup.Size);
        }

        [Fact]
        public void MoveTask_Success()
        {
            const string taskGroupName = "New Task Group";
            mTaskManager.CreateNewTaskGroup(taskGroupName);
            ITasksGroup taskGroup = mTaskManager.GetAllTasksGroups().First(group => group.Name == taskGroupName);
            Assert.Equal(0, taskGroup.Size);

            // Create new task in free task group.
            const string taskDescription = "new task";
            mTaskManager.CreateNewTask(taskDescription);
            Assert.Equal(1, mTaskManager.FreeTasksGroup.Size);

            IWorkTask taskToMove = mTaskManager.GetAllTasks(task => task.Description == taskDescription).First();
            mTaskManager.MoveTaskToGroup(taskToMove.ID, taskGroupName);

            Assert.Equal(1, taskGroup.Size);
            Assert.Equal(0, mTaskManager.FreeTasksGroup.Size);
        }
    }
}