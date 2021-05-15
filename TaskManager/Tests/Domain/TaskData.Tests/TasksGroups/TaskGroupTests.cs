using Microsoft.Extensions.Logging.Abstractions;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using TaskData.WorkTasks.Producers;
using Xunit;

namespace TaskData.Tests.TasksGroups
{
    public class TaskGroupTests
    {
        private readonly TaskGroupFactory mTaskGroupFactory;
        private readonly WorkTaskProducer mWorkTaskProducer = new WorkTaskProducer();
        private readonly TasksGroupProducer mTasksGroupProducer = new TasksGroupProducer();

        public TaskGroupTests()
        {
            IDProducer idProducer = new IDProducer();
            mTaskGroupFactory = new TaskGroupFactory(idProducer, NullLogger<TaskGroupFactory>.Instance);
        }

        [Fact]
        public void Size_NumberOfTasks3()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup", mTasksGroupProducer).Value;
            mTaskGroupFactory.CreateTask(taskGroup, "task1", mWorkTaskProducer);
            mTaskGroupFactory.CreateTask(taskGroup, "task2", mWorkTaskProducer);
            mTaskGroupFactory.CreateTask(taskGroup, "task3", mWorkTaskProducer);

            Assert.Equal(3, taskGroup.Size);
        }

        [Fact]
        public void IsFinished_HasOpenTasks_False()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup", mTasksGroupProducer).Value;

            mTaskGroupFactory.CreateTask(taskGroup, "task1", mWorkTaskProducer);
            IWorkTask task2 = mTaskGroupFactory.CreateTask(taskGroup, "task2", mWorkTaskProducer).Value;
            IWorkTask task3 = mTaskGroupFactory.CreateTask(taskGroup, "task3", mWorkTaskProducer).Value;

            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.False(taskGroup.IsFinished);
        }

        [Fact]
        public void IsFinished_HasNoOpenTasks_True()
        {
            ITasksGroup taskGroup = mTaskGroupFactory.CreateGroup("TestGroup", mTasksGroupProducer).Value;

            IWorkTask task1 = mTaskGroupFactory.CreateTask(taskGroup, "task1", mWorkTaskProducer).Value;
            IWorkTask task2 = mTaskGroupFactory.CreateTask(taskGroup, "task2", mWorkTaskProducer).Value;
            IWorkTask task3 = mTaskGroupFactory.CreateTask(taskGroup, "task3", mWorkTaskProducer).Value;

            task1.CloseTask(string.Empty);
            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.True(taskGroup.IsFinished);
        }
    }
}