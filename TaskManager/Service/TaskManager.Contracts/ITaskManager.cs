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
          IEnumerable<ITaskGroup> GetAllTasksGroups();

          void CreateNewTask(ITaskGroup tasksGroup, string description);
          void CreateNewTaskByGroupName(string tasksGroupName, string description);
          void CreateNewTaskByGroupId(string tasksGroupId, string description);
          void CreateNewTask(string description);
          IEnumerable<ITask> GetAllTasks();
          IEnumerable<ITask> GetAllTasks(ITaskGroup taskGroup);
          IEnumerable<ITask> GetAllTasksByGroupName(string taskGroupName);
          IEnumerable<ITask> GetAllTasksByGroupId(string taskGroupId);

          void ChangeDatabasePath(string newDatabasePath);
     }
}