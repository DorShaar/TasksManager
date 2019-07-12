﻿using Database.Contracts;
using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
          private readonly ITaskGroupBuilder mTaskGroupBuilder;

          internal static readonly string FreeTaskGroupName = "Free";

          public TaskManager(IRepository<ITaskGroup> database, ITaskGroupBuilder taskGroupBuilder, ILogger logger)
          {
               mLogger = logger;
               mDatabase = database;
               mTaskGroupBuilder = taskGroupBuilder;
               InitializeFreeTasksGroup();
          }

          private void InitializeFreeTasksGroup()
          {
               mFreeTasksGroup = mDatabase.GetByName(FreeTaskGroupName);

               if (mFreeTasksGroup == null)
               {
                    mFreeTasksGroup = mTaskGroupBuilder.Create(FreeTaskGroupName, mLogger);
                    mDatabase.Insert(mFreeTasksGroup);
               }
          }

          /// <summary>
          /// Create new task group.
          /// </summary>
          public void CreateNewTaskGroup(string groupName)
          {
               mDatabase.Insert(mTaskGroupBuilder.Create(groupName, mLogger));
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
                    allTasks = allTasks.Concat(taskGroup.GetAllTasks());
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

          public void RemoveTask(string taskId)
          {
               foreach(ITaskGroup group in mDatabase.GetAll())
               {
                    foreach(ITask task in group.GetAllTasks())
                    {
                         if (task.ID == taskId)
                         {
                              group.RemoveTask(taskId);
                              mDatabase.Update(group);
                              return;
                         }
                    }
               }

               mLogger.LogError($"Task id {taskId} was not found");
          }

          public void RemoveTask(string[] taskIds)
          {
               foreach(string taskId in taskIds)
               {
                    RemoveTask(taskId);
               }
          }

          public void MoveTaskToGroupName(string taskId, string taskGroupName)
          {
               ITaskGroup taskGroup = mDatabase.GetByName(taskGroupName);
               if(taskGroup == null)
               {
                    mLogger.LogError($"group name {taskGroupName} was not found");
                    return;
               }

               MoveTaskToGroup(taskId, taskGroup);
          }

          public void MoveTaskToGroupId(string taskId, string taskGroupId)
          {
               ITaskGroup taskGroup = mDatabase.GetById(taskGroupId);
               if (taskGroup == null)
               {
                    mLogger.LogError($"group id {taskGroupId} was not found");
                    return;
               }

               MoveTaskToGroup(taskId, taskGroup);
          }

          private void MoveTaskToGroup(string taskId, ITaskGroup taskGroupDestination)
          {
               foreach (ITaskGroup sourceGroup in mDatabase.GetAll())
               {
                    foreach (ITask task in sourceGroup.GetAllTasks())
                    {
                         if (task.ID == taskId)
                         {
                              taskGroupDestination.AddTask(task);
                              mDatabase.Update(taskGroupDestination);

                              sourceGroup.RemoveTask(taskId);
                              mDatabase.Update(sourceGroup);
                              return;
                         }
                    }
               }

               mLogger.LogError($"Task id {taskId} was not found");
          }

          public void ChangeDatabasePath(string newDatabasePath)
          {
               mDatabase.SetDatabasePath(newDatabasePath);
          }
     }
}