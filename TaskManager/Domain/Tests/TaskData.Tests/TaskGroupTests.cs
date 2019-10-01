using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskData.Contracts;

namespace TaskData.Tests
{
    [TestClass]
    public class TaskGroupTests
    {
        private readonly ILogger mLogger = A.Dummy<ILogger>();
        private readonly TaskGroupBuilder taskGroupBuilder = new TaskGroupBuilder();

        [TestMethod]
        public void Size_NumberOfTasks3()
        {
            ITaskGroup taskGroup = taskGroupBuilder.Create("TestGroup", mLogger);
            taskGroup.CreateTask("1");
            taskGroup.CreateTask("2");
            taskGroup.CreateTask("3");

            Assert.AreEqual(3, taskGroup.Size);
        }

        [TestMethod]
        public void IsFinished_HasOpenTasks_False()
        {
            ITaskGroup taskGroup = taskGroupBuilder.Create("TestGroup", mLogger);
            taskGroup.CreateTask("1");
            ITask task2 = taskGroup.CreateTask("2");
            ITask task3 = taskGroup.CreateTask("3");

            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.IsFalse(taskGroup.IsFinished);
        }

        [TestMethod]
        public void IsFinished_HasNoOpenTasks_True()
        {
            ITaskGroup taskGroup = taskGroupBuilder.Create("TestGroup", mLogger);
            ITask task1 = taskGroup.CreateTask("1");
            ITask task2 = taskGroup.CreateTask("2");
            ITask task3 = taskGroup.CreateTask("3");

            task1.CloseTask(string.Empty);
            task2.CloseTask(string.Empty);
            task3.CloseTask(string.Empty);

            Assert.IsTrue(taskGroup.IsFinished);
        }
    }
}