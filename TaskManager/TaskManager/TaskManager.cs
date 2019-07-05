using Database.Contracts;
using System.Collections.Generic;
using System.Linq;
using TaskData;
using TaskData.Contracts;

namespace TaskManager
{
     public class TaskManager
     {
          private const string FreeTaskGroupName = "Free";

          private ITaskGroup mFreeTasksGroup;
          private readonly IRepository<ITaskGroup> mDatabase;

          public TaskManager(IRepository<ITaskGroup> database)
          {
               mDatabase = database;
               InitializeFreeTasksGroup();
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