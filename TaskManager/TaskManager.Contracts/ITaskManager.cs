using System.Collections.Generic;
using TaskData.Contracts;

namespace TaskManager.Contracts
{
     public interface ITaskManager
     {
          void CreateNewTaskGroup(string groupName);
          void RemoveTaskGroup(ITaskGroup taskGroup);
          void RemoveTaskGroupByName(string name);
          void RemoveTaskGroupById(string id);
          void CreateNewTask(ITaskGroup tasksGroup, string description);
          void CreateNewTask(string description);
          IEnumerable<ITask> GetAllTasks();
          IEnumerable<ITask> GetAllTasks(ITaskGroup taskGroup);
          IEnumerable<ITask> GetAllTasks(string taskGroupName);
          IEnumerable<ITaskGroup> GetAllTasksGroups();
     }
}