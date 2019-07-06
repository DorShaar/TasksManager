using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TaskData;
using TaskData.Contracts;

namespace TaskManager.Integration.Tests
{
     [TestClass]
     public class TaskManagerTests
     {
          private static readonly string DatabasePath = "dummyTasks.db";
          private static readonly ILogger mLogger = A.Dummy<ILogger>();
          private readonly TaskManager mTaskManager = new TaskManager(DatabasePath, mLogger);

          [TestMethod]
          public void Ctor_TasksManagerHasFreeTasksGroup()
          {
               Assert.IsNotNull(mTaskManager.GetAllTasksByGroupName(TaskManager.FreeTaskGroupName));
          }

          [TestMethod]
          public void CreateNewTask_AddNewTaskToGroup_Success()
          {
               ITaskGroup taskGroup = new TaskGroup("A");
               mTaskManager.CreateNewTask(taskGroup, "New Task Group");
               Assert.AreEqual(mTaskManager.GetAllTasksGroups().Count(), 2);
          }

          [TestMethod]
          public void CreateNewTask_AddNewTaskToFreeGroup_Success()
          {
               mTaskManager.CreateNewTask("New Task Group");
               Assert.AreEqual(mTaskManager.GetAllTasksGroups().Count(), 1);
          }

          [TestMethod]
          public void RemoveTask_ExistingGroup_Success()
          {
               string taskGroupName = "New Task Group";
               mTaskManager.CreateNewTaskGroup(taskGroupName);
               Assert.AreEqual(mTaskManager.GetAllTasksByGroupName(taskGroupName).Count(), 0);

               mTaskManager.RemoveTaskGroupByName(taskGroupName);
               Assert.IsNull(mTaskManager.GetAllTasksByGroupName(taskGroupName));
          }
     }
}