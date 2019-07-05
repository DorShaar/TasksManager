﻿using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.Contracts;

[assembly: InternalsVisibleTo("Statistics")]
namespace TaskData
{
     public class TaskGroup : ITaskGroup
     {
          private static ILogger mLogger;
          internal readonly Dictionary<string, ITask> mTasksChildren = new Dictionary<string, ITask>();

          public string ID { get; } = IDCounter.GetNextID();
          public string GroupName { get; set ; }

          public int Size => mTasksChildren.Count;

          public TaskGroup(string groupName)
          {
               GroupName = groupName;
          }

          public static void SetLogger(ILogger logger)
          {
               mLogger = logger;
          }

          public void CreateTask(string description)
          {
               Task createdTask = new Task(description);
               AddTask(createdTask);
          }

          public IEnumerable<ITask> GetAllTasks()
          {
               return mTasksChildren.Values.AsEnumerable();
          }

          public ITask GetTask(string id)
          {
               if (!mTasksChildren.TryGetValue(id, out ITask task))
                    mLogger?.Log($"Task id {id} was not found in group {GroupName}");

               return task;
          }

          public void AddTask(ITask task)
          {
               if(mTasksChildren.ContainsKey(task.ID))
               {
                    mLogger?.Log($"Task id {task.ID} is already found in group {GroupName}");
                    return;
               }

               mTasksChildren.Add(task.ID, task);
               mLogger?.Log($"Task id {task.ID} added to group {GroupName}");
          }

          public void RemoveTask(string id)
          {
               if (!mTasksChildren.ContainsKey(id))
               {
                    mLogger?.Log($"Task id {id} was not found in group {GroupName}");
                    return;
               }

               mTasksChildren.Remove(id);
               mLogger?.Log($"Task id {id} removed from group {GroupName}");
          }

          public void RemoveTask(params string[] ids)
          {
               foreach(string id in ids)
               {
                    RemoveTask(id);
               }
          }

          public void UpdateTask(ITask task)
          {
               if (!mTasksChildren.ContainsKey(task.ID))
               {
                    mLogger?.LogError($"Task id {task.ID} is not in group {GroupName}. Update failed");
                    return;
               }

               mTasksChildren[task.ID] = task;
               mLogger?.LogError($"Task id {task.ID} updated in group {GroupName}. Update failed");
          }
     }
}