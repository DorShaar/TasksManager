using Logger.Contracts;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaskData.Contracts;

[assembly: InternalsVisibleTo("Statistics")]
namespace TaskData
{
     public class TaskGroup : ITaskGroup
     {
          private readonly ILogger mLogger;
          internal readonly Dictionary<string, ITask> mTasksChildren = new Dictionary<string, ITask>();

          public string Name { get; set ; }

          public int Size => mTasksChildren.Count;

          public TaskGroup(ILogger logger)
          {
               mLogger = logger;
          }

          public ITask GetTask(string id)
          {
               if (!mTasksChildren.TryGetValue(id, out ITask task))
                    mLogger.Log($"Task id {id} was not found in group {Name}");

               return task;
          }

          public void AddTask(ITask task)
          {
               if(mTasksChildren.ContainsKey(task.ID))
               {
                    mLogger.Log($"Task id {task.ID} is already found in group {Name}");
                    return;
               }

               mTasksChildren.Add(task.ID, task);
               mLogger.Log($"Task id {task.ID} added to group {Name}");
          }

          public void RemoveTask(string id)
          {
               if (!mTasksChildren.ContainsKey(id))
               {
                    mLogger.Log($"Task id {id} was not found in group {Name}");
                    return;
               }

               mTasksChildren.Remove(id);
               mLogger.Log($"Task id {id} removed from group {Name}");
          }

          public void UpdateTask(ITask task)
          {
               if (!mTasksChildren.ContainsKey(task.ID))
               {
                    mLogger.LogError($"Task id {task.ID} is not in group {Name}. Update failed");
                    return;
               }

               mTasksChildren[task.ID] = task;
               mLogger.LogError($"Task id {task.ID} updated in group {Name}. Update failed");
          }
     }
}