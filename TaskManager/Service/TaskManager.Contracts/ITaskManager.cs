using System;
using System.Collections.Generic;
using TaskData.Contracts;

namespace TaskManager.Contracts
{
     public interface ITaskManager
     {
          // TasksGroups.
          void CreateNewTaskGroup(string groupName);
          void RemoveTaskGroupByName(string name, bool shouldMoveInnerTasks);
          void RemoveTaskGroupById(string id, bool shouldMoveInnerTasks);
          IEnumerable<ITaskGroup> GetAllTasksGroups();

          // Tasks.
          ITask CreateNewTask(ITaskGroup tasksGroup, string description);
          void CreateNewTaskByGroupName(string tasksGroupName, string description);
          void CreateNewTaskByGroupId(string tasksGroupId, string description);
          void CreateNewTask(string description);
          IEnumerable<ITask> GetAllTasks();
          IEnumerable<ITask> GetAllTasks(Func<ITaskGroup, bool> action);
          IEnumerable<ITask> GetAllTasks(Func<ITask, bool> action);
          void CloseTask(string taskId);
          void ReOpenTask(string taskId);
          void RemoveTask(string taskId);
          void MoveTaskToGroupName(string taskId, string taskGroupName);
          void MoveTaskToGroupId(string taskId, string taskGroupId);

          // Notes.
          void CreateNote(string taskId, string content);
          void OpenNote(string taskId);
          string GetNote(string taskId);

          // Database.
          void ChangeDatabasePath(string newDatabasePath);
     }
}