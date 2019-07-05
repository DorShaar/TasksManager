using Database.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskManager.Integration.Tests
{
     [TestClass]
     public class TaskManagerTests
     {
          private readonly IRepository mDatabase;
          private readonly TaskManager taskManager = new TaskManager();

          [TestMethod]
          public void TestMethod1()
          {
          }

          private void InitializeFreeTasksGroup()
          {
               mFreeTasksGroup = mDatabase.GetByName(FreeTaskGroupName);

               if (mFreeTasksGroup == null)
                    mFreeTasksGroup = new TaskGroup(FreeTaskGroupName);
          }

          /// <summary>
          /// Create new task into <param name="tasksGroup"/>.
          /// </summary>
          public void CreateNewTask(ITaskGroup tasksGroup, string description)
          {
               tasksGroup.CreateTask(description);
               mDatabase.Update(tasksGroup);
          }

          /// <summary>
          /// Create new task into <see cref="mFreeTasksGroup"/>.
          /// </summary>
          public void CreateNewTask(string description)
          {
               mFreeTasksGroup.CreateTask(description);
               mDatabase.Update(mFreeTasksGroup);
          }

          public IEnumerable<ITask> GetAllTasks()
          {
               IEnumerable<ITask> allTasks = new List<ITask>();

               foreach (ITaskGroup taskGroup in GetAllTasksGroups())
               {
                    allTasks.Concat(taskGroup.GetAllTasks());
               }

               return allTasks;
          }

          public IEnumerable<ITask> GetAllTasks(ITaskGroup taskGroup)
          {
               return taskGroup.GetAllTasks();
          }

          private IEnumerable<ITaskGroup> GetAllTasksGroups()
          {
               return mDatabase.GetAll();
          }
     }
}