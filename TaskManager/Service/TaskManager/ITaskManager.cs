using System;
using System.Collections.Generic;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.WorkTasks;

namespace TaskManagers
{
    public interface ITaskManager
    {
        // TasksGroups.
        void CreateNewTaskGroup(string groupName);
        void RemoveTaskGroup(string taskGroup, bool shouldMoveInnerTasks);
        IEnumerable<ITasksGroup> GetAllTasksGroups();
        ITasksGroup DefaultTaskGroupName { get; }

        // Tasks.
        IWorkTask CreateNewTask(ITasksGroup tasksGroup, string description);
        void CreateNewTask(string description);
        IEnumerable<IWorkTask> GetAllTasks();
        IEnumerable<IWorkTask> GetAllTasks(Func<ITasksGroup, bool> action);
        IEnumerable<IWorkTask> GetAllTasks(Func<IWorkTask, bool> action);
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
    }
}