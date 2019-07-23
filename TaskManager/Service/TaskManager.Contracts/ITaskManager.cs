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
        ITask CreateNewTask(ITaskGroup tasksGroup, string description);
        void CreateNewTask(string description);
        IEnumerable<ITask> GetAllTasks();
        IEnumerable<ITask> GetAllTasks(Func<ITaskGroup, bool> action);
        IEnumerable<ITask> GetAllTasks(Func<ITask, bool> action);
        void CloseTask(string taskId);
        void ReOpenTask(string taskId);
        void MarkTaskOnWork(string taskId);
        void RemoveTask(string taskId);
        void MoveTaskToGroup(string taskId, string taskGroup);

        // Notes.
        void CreateNote(string taskId, string content);
        void CreateGeneralNote(string taskSubject, string content);
        void OpenNote(string taskId);
        IEnumerable<INote> GetNotes();
        string GetNote(string taskId);

        // Database.
        string GetDatabasePath();
        void ChangeDatabasePath(string newDatabasePath);
    }
}