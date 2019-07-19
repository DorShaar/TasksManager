using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskData.Contracts;

namespace TaskGroup.Tests
{
    [TestClass]
    public class TaskGroupTests
    {
        [TestMethod]
        public void Size_NumberOfTasks3()
        {
            ITaskGroup taskGroup = new TaskGroup()
            ITask task1 = mTaskManager.CreateNewTask(taskGroup, "1");
            ITask task2 = mTaskManager.CreateNewTask(taskGroup, "2");
            ITask task3 = mTaskManager.CreateNewTask(taskGroup, "3");
        }

        [TestMethod]
        public void IsFinished_HasOpenTasks_False()
        {

        }

        [TestMethod]
        public void IsFinished_HasNoOpenTasks_True()
        {

        }
    }
}