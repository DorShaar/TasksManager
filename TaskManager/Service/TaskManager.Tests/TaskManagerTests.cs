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
          private static readonly IRepository<ITaskGroup> mDatabase = new Database<ITaskGroup>(mConfiguration, mSerializer, mLogger);
          private static readonly ITaskGroupBuilder mTaskGroupBuilder = new TaskGroupBuilder();
          private readonly TaskManager mTaskManager = new TaskManager(mDatabase, mTaskGroupBuilder, mLogger);

          [TestMethod]
          public void Ctor_TasksManagerHasFreeTasksGroup()
          {
               Assert.IsNotNull(mTaskManager.GetAllTasksByGroupName(TaskManager.FreeTaskGroupName));
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
               Assert.AreEqual(mTaskManager.GetAllTasksByGroupName(taskGroupName).Count(), 0);

               mTaskManager.RemoveTaskGroupByName(taskGroupName);
               Assert.IsNull(mTaskManager.GetAllTasksByGroupName(taskGroupName));
          }
     }
}