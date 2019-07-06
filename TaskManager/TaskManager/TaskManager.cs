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

          private void InitializeFreeTasksGroup()
          {
               mFreeTasksGroup = mDatabase.GetByName(FreeTaskGroupName);

               if (mFreeTasksGroup == null)
               {
                    mFreeTasksGroup = new TaskGroup(FreeTaskGroupName, mLogger);
                    mDatabase.Insert(mFreeTasksGroup);
               }
          }

          /// <summary>
          /// Create new task group.
          /// </summary>
          public void CreateNewTaskGroup(string groupName)
          {
               mDatabase.Insert(new TaskGroup(groupName, mLogger));
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

          public IEnumerable<ITaskGroup> GetAllTasksGroups()
          {
               return mDatabase.GetAll();
          }

          /// <summary>
          /// Create new task into <param name="tasksGroup"/>.
          /// </summary>
          public void CreateNewTask(ITaskGroup tasksGroup, string description)
          {
               tasksGroup.CreateTask(description);
               mDatabase.AddOrUpdate(tasksGroup);
          }

          public void CreateNewTaskByGroupName(string tasksGroupName, string description)
          {
               ITaskGroup taskGroup = mDatabase.GetByName(tasksGroupName);
               CreateNewTask(taskGroup, description);
          }

          public void CreateNewTaskByGroupId(string tasksGroupId, string description)
          {
               ITaskGroup taskGroup = mDatabase.GetById(tasksGroupId);
               CreateNewTask(taskGroup, description);
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

          public IEnumerable<ITask> GetAllTasksByGroupName(string taskGroupName)
          {
               return GetAllTasks(
                    GetAllTasksGroups().FirstOrDefault(group => group.GroupName == taskGroupName));
          }

          public IEnumerable<ITask> GetAllTasksByGroupId(string taskGroupId)
          {
               return GetAllTasks(
                    GetAllTasksGroups().FirstOrDefault(group => group.ID == taskGroupId));
          }
     }
}