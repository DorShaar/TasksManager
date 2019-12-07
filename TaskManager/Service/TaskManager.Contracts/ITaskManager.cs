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
        string DefaultTaskGroupName { get; }

        // Tasks.
        ITask CreateNewTask(ITaskGroup tasksGroup, string description);
        void CreateNewTask(string description);
        IEnumerable<ITask> GetAllTasks();
        IEnumerable<ITask> GetAllTasks(Func<ITaskGroup, bool> action);
        IEnumerable<ITask> GetAllTasks(Func<ITask, bool> action);
        void CloseTask(string taskId, string reason);
        void ReOpenTask(string taskId, string reason);
        void MarkTaskOnWork(string taskId, string reason);
        void RemoveTask(string taskId);
        void MoveTaskToGroup(string taskId, string taskGroup);

        // Notes.
        INotesSubject NotesRootDatabase { get; }
        INotesSubject NotesTasksDatabase { get; }
        IEnumerable<INote> GetAllNotes();
        void CreateTaskNote(string taskId, string content);
        void CreateGeneralNote(string taskSubject, string content);

        // Database.
        string GetDatabasePath();

        // TODO DELETE
        void Fix();
    }
}