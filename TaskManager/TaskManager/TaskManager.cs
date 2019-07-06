using Database.Contracts;
using Database.JsonService;
using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData;
using TaskData.Contracts;
using TaskManager.Contracts;

[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
namespace TaskManager
{
     public class TaskManager : ITaskManager
     {
          private readonly ILogger mLogger;
          private ITaskGroup mFreeTasksGroup;
          private readonly IRepository<ITaskGroup> mDatabase;

          internal static readonly string FreeTaskGroupName = "Free";

          public TaskManager(IRepository<ITaskGroup> database, ILogger logger)
          {
               mLogger = logger;
               mDatabase = database;
               InitializeFreeTasksGroup();
          }

          public TaskManager(string databasePath, ILogger logger) :
                        this(new Database<ITaskGroup>(databasePath, logger), logger)
          {
          }

          private void InitializeFreeTasksGroup()
          {
               mFreeTasksGroup = mDatabase.GetByName(FreeTaskGroupName);

               if (mFreeTasksGroup == null)
               {
                    mFreeTasksGroup = new TaskGroup(FreeTaskGroupName);
                    mDatabase.Insert(mFreeTasksGroup);
               }
          }

          /// <summary>
          /// Create new task group.
          /// </summary>
          public void CreateNewTaskGroup(string groupName)
          {
               mDatabase.Insert(new TaskGroup(groupName));
          }

          public void RemoveTaskGroup(ITaskGroup taskGroup)
          {
               if (taskGroup == mFreeTasksGroup)
               {
                    mLogger.LogError($"Cannot delete {FreeTaskGroupName} from database");
                    return;
               }

               mDatabase.Remove(taskGroup);
          }

          public void RemoveTaskGroupByName(string name)
          {
               ITaskGroup taskGroup = mDatabase.GetByName(name);
               RemoveTaskGroup(taskGroup);
          }

          public void RemoveTaskGroupById(string id)
          {
               ITaskGroup taskGroup = mDatabase.GetById(id);
               RemoveTaskGroup(taskGroup);
          }

          /// <summary>
          /// Create new task into <param name="tasksGroup"/>.
          /// </summary>
          public void CreateNewTask(ITaskGroup tasksGroup, string description)
          {
               tasksGroup.CreateTask(description);
               mDatabase.AddOrUpdate(tasksGroup);
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
               return taskGroup?.GetAllTasks();
          }

          public IEnumerable<ITask> GetAllTasks(string taskGroupName)
          {
               return GetAllTasks(
                    GetAllTasksGroups().FirstOrDefault(group => group.GroupName == taskGroupName));
          }

          public IEnumerable<ITaskGroup> GetAllTasksGroups()
          {
               return mDatabase.GetAll();
          }
     }
}