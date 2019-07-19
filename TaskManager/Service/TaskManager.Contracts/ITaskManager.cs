using System;
using System.Collections.Generic;
using TaskData.Contracts;

namespace TaskManager.Contracts
{
     public interface ITaskManager
     {
          // TasksGroups.
          void CreateNewTaskGroup(string groupName);
          void RemoveTaskGroup(string taskGroup, bool shouldMoveInnerTasks);
          IEnumerable<ITaskGroup> GetAllTasksGroups();

          // Tasks.
          ITask CreateNewTask(string tasksGroup, string description);
          void CreateNewTask(string description);
          IEnumerable<ITask> GetAllTasks();
          IEnumerable<ITask> GetAllTasks(Func<ITaskGroup, bool> action);
          IEnumerable<ITask> GetAllTasks(Func<ITask, bool> action);
          void CloseTask(string taskId);
          void ReOpenTask(string taskId);
          void RemoveTask(string taskToRemove);
          void MoveTaskToGroup(string taskId, string taskGroup);

          // Notes.
          void CreateNote(string taskId, string content);
          void OpenNote(string taskId);
          string GetNote(string taskId);

          // Database.
          void ChangeDatabasePath(string newDatabasePath);
     }
}