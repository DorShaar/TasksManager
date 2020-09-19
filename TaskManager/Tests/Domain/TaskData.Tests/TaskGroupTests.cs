using Microsoft.Extensions.Logging.Abstractions;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Xunit;

namespace TaskData.Tests
{
    public class TaskGroupTests
    {
        private readonly TaskGroupFactory mTaskGroupFactory;
        public TaskGroupTests()
        {
            IDProducer idProducer = new IDProducer();
            mTaskGroupFactory = new TaskGroupFactory(idProducer, NullLogger< TaskGroupFactory>.Instance);
        }

        [Fact]
        public void Size_NumberOfTasks3()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup");
            taskGroup.CreateTask("1001", "task1");
            taskGroup.CreateTask("1002", "task2");
            taskGroup.CreateTask("1003", "task3");

            Assert.Equal(3, taskGroup.Size);
        }

        [Fact]
        public void IsFinished_HasOpenTasks_False()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup");
            taskGroup.CreateTask("1001", "task1");
            IWorkTask task2 = taskGroup.CreateTask("1002", "task2").Value;
            IWorkTask task3 = taskGroup.CreateTask("1003", "task3").Value;

            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.False(taskGroup.IsFinished);
        }

        [Fact]
        public void IsFinished_HasNoOpenTasks_True()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup");
            IWorkTask task1 = taskGroup.CreateTask("1001", "task1").Value;
            IWorkTask task2 = taskGroup.CreateTask("1002", "task2").Value;
            IWorkTask task3 = taskGroup.CreateTask("1003", "task3").Value;

            task1.CloseTask(string.Empty);
            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.True(taskGroup.IsFinished);
        }
    }
}