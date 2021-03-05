using Microsoft.Extensions.Logging.Abstractions;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Xunit;

namespace TaskData.Tests.TasksGroups
{
    public class TaskGroupTests
    {
        private readonly TaskGroupFactory mTaskGroupFactory;
        public TaskGroupTests()
        {
            IDProducer idProducer = new IDProducer();
            mTaskGroupFactory = new TaskGroupFactory(idProducer, new WorkTaskProducer(), NullLogger<TaskGroupFactory>.Instance);
        }

        [Fact]
        public void Size_NumberOfTasks3()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup").Value;
            mTaskGroupFactory.CreateTask(taskGroup, "task1");
            mTaskGroupFactory.CreateTask(taskGroup, "task2");
            mTaskGroupFactory.CreateTask(taskGroup, "task3");

            Assert.Equal(3, taskGroup.Size);
        }

        [Fact]
        public void IsFinished_HasOpenTasks_False()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup").Value;

            mTaskGroupFactory.CreateTask(taskGroup, "task1");
            IWorkTask task2 = mTaskGroupFactory.CreateTask(taskGroup, "task2").Value;
            IWorkTask task3 = mTaskGroupFactory.CreateTask(taskGroup, "task3").Value;

            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.False(taskGroup.IsFinished);
        }

        [Fact]
        public void IsFinished_HasNoOpenTasks_True()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup").Value;

            IWorkTask task1 = mTaskGroupFactory.CreateTask(taskGroup, "task1").Value;
            IWorkTask task2 = mTaskGroupFactory.CreateTask(taskGroup, "task2").Value;
            IWorkTask task3 = mTaskGroupFactory.CreateTask(taskGroup, "task3").Value;

            task1.CloseTask(string.Empty);
            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.True(taskGroup.IsFinished);
        }
    }
}