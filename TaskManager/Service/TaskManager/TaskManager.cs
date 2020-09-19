using Databases;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.Notes;
using TaskData.OperationResults;
using TaskData.TasksGroups;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskManagers
{
    internal class TaskManager : ITaskManager
    {
        internal const string FreeTaskGroupName = "Free";

        private readonly ILogger<TaskManager> mLogger;
        private readonly INoteFactory mNoteFactory;
        private readonly ITasksGroupFactory mTaskGroupFactory;
        private readonly ILocalRepository<ITasksGroup> mTasksDatabase;

        public INotesSubject NotesRootDatabase { get; }
        public INotesSubject NotesTasksDatabase { get; }
        public ITasksGroup FreeTasksGroup { get; private set; }
        public ITasksGroup DefaultTaskGroupName { get; }

        public TaskManager(
            ILocalRepository<ITasksGroup> tasksDatabase,
            ITasksGroupFactory taskGroupFactory,
            INoteFactory noteFactory,
            ILogger<TaskManager> logger)
        {
            mTasksDatabase = tasksDatabase ?? throw new ArgumentNullException(nameof(tasksDatabase));
            mTaskGroupFactory = taskGroupFactory ?? throw new ArgumentNullException(nameof(taskGroupFactory));
            mNoteFactory = noteFactory ?? throw new ArgumentNullException(nameof(noteFactory));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            NotesRootDatabase = noteFactory.LoadNotesSubject(mTasksDatabase.NotesDirectoryPath);
            NotesTasksDatabase = noteFactory.LoadNotesSubject(mTasksDatabase.NotesTasksDirectoryPath);
            DefaultTaskGroupName = mTasksDatabase.GetEntity(mTasksDatabase.DefaultTasksGroup);

            InitializeFreeTasksGroup();
        }

        private void InitializeFreeTasksGroup()
        {
            FreeTasksGroup = mTasksDatabase.GetEntity(FreeTaskGroupName);

            if (FreeTasksGroup == null)
            {
                FreeTasksGroup = mTaskGroupFactory.CreateGroup(FreeTaskGroupName);
                mTasksDatabase.Insert(FreeTasksGroup);
            }
        }

        public void CreateNewTaskGroup(string groupName)
        {
            mTasksDatabase.Insert(mTaskGroupFactory.CreateGroup(groupName));
        }

        public void RemoveTaskGroup(string tasksGroup, bool shouldMoveInnerTasks)
        {
            ITasksGroup taskGroup = mTasksDatabase.GetEntity(tasksGroup);
            if (taskGroup == null)
            {
                mLogger.LogError($"Task group {tasksGroup} does not exists");
                return;
            }

            if (taskGroup == FreeTasksGroup)
            {
                mLogger.LogError($"Cannot delete {FreeTaskGroupName} from database");
                return;
            }

            if (shouldMoveInnerTasks)
            {
                // Should iterate with for and not for each because we are changing the size of the IEnumarable.
                IWorkTask[] tasksToMove = taskGroup.GetAllTasks().ToArray();
                for (int i = 0; i < tasksToMove.Length; ++i)
                {
                    MoveTaskToGroup(tasksToMove[i].ID, FreeTasksGroup);
                }
            }

            mTasksDatabase.Remove(taskGroup);
        }

        public IEnumerable<ITasksGroup> GetAllTasksGroups()
        {
            return mTasksDatabase.GetAll();
        }

        /// <summary>
        /// Create new task into <param name="tasksGroup"/>.
        /// </summary>
        public IWorkTask CreateNewTask(ITasksGroup tasksGroup, string description)
        {
            if (tasksGroup == null)
            {
                mLogger.LogError("Given task group is null");
                return null;
            }

            IWorkTask workTask = mTaskGroupFactory.CreateTask(tasksGroup, description);

            mTasksDatabase.AddOrUpdate(tasksGroup);

            return workTask;
        }

        /// <summary>
        /// Create new task into <see cref="FreeTasksGroup"/>.
        /// </summary>
        public void CreateNewTask(string description)
        {
            mTaskGroupFactory.CreateTask(FreeTasksGroup, description);
            mTasksDatabase.Update(FreeTasksGroup);
        }

        public IEnumerable<IWorkTask> GetAllTasks()
        {
            IEnumerable<IWorkTask> allTasks = new List<IWorkTask>();

            foreach (ITasksGroup taskGroup in GetAllTasksGroups())
            {
                allTasks = allTasks.Concat(taskGroup.GetAllTasks());
            }

            return allTasks;
        }

        public IEnumerable<IWorkTask> GetAllTasks(Func<IWorkTask, bool> action)
        {
            foreach (IWorkTask task in GetAllTasks())
            {
                if (action(task))
                    yield return task;
            }
        }

        public IEnumerable<IWorkTask> GetAllTasks(Func<ITasksGroup, bool> action)
        {
            foreach (ITasksGroup taskGroup in GetAllTasksGroups())
            {
                if (action(taskGroup))
                {
                    return taskGroup.GetAllTasks();
                }
            }

            return null;
        }

        public void CloseTask(string taskId, string reason)
        {
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId).Value;
                if (task != null)
                {
                    OperationResult closeTaskResult = task.CloseTask(reason);
                    closeTaskResult.Log(mLogger);

                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void ReOpenTask(string taskId, string reason)
        {
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId).Value;
                if (task != null)
                {
                    OperationResult reopenResult = task.ReOpenTask(reason);
                    reopenResult.Log(mLogger);

                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void MarkTaskOnWork(string taskId, string reason)
        {
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId).Value;
                if (task != null)
                {
                    OperationResult markTaskResult = task.MarkTaskOnWork(reason);
                    markTaskResult.Log(mLogger);

                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void RemoveTask(string taskId)
        {
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId).Value;
                if (task != null)
                {
                    OperationResult removeTaskResult = group.RemoveTask(taskId);
                    removeTaskResult.Log(mLogger);

                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task {taskId} was not found");
        }

        public void MoveTaskToGroup(string taskId, string taskGroup)
        {
            ITasksGroup taskGroupDestination = mTasksDatabase.GetEntity(taskGroup);
            if (taskGroup == null)
            {
                mLogger.LogError($"Task group {taskGroup} was not found");
                return;
            }

            MoveTaskToGroup(taskId, taskGroupDestination);
        }

        private void MoveTaskToGroup(string taskId, ITasksGroup destionationTaskGroup)
        {
            if (destionationTaskGroup == null)
            {
                mLogger.LogError("Destination task group was not found");
                return;
            }

            foreach (ITasksGroup sourceGroup in mTasksDatabase.GetAll())
            {
                IWorkTask task = sourceGroup.GetTask(taskId).Value;
                if (task != null)
                {
                    ITasksGroup sourceTaskGroup = mTasksDatabase.GetEntity(task.GroupName);

                    if (sourceTaskGroup == null)
                    {
                        mLogger.LogError("Moving task failed since source group is not given, unable checking moving to same group");
                        return;
                    }

                    if (sourceTaskGroup != destionationTaskGroup)
                    {
                        OperationResult addTaskResult = destionationTaskGroup.AddTask(task);
                        addTaskResult.Log(mLogger);
                        mTasksDatabase.Update(destionationTaskGroup);

                        OperationResult removeTaskResult = sourceGroup.RemoveTask(taskId);
                        removeTaskResult.Log(mLogger);
                        mTasksDatabase.Update(sourceGroup);
                        return;
                    }
                    else
                    {
                        mLogger.LogError("Moving task failed since Source group and destination group are the same");
                        return;
                    }
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateTaskNote(string taskId, string content)
        {
            foreach (ITasksGroup taskGroup in mTasksDatabase.GetAll())
            {
                IWorkTask task = taskGroup.GetTask(taskId).Value;
                if (task != null)
                {
                    OperationResult createNoteResult = task.CreateNote(NotesTasksDatabase.NoteSubjectFullPath, content);
                    createNoteResult.Log(mLogger);

                    mTasksDatabase.Update(taskGroup);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateGeneralNote(string taskSubject, string content)
        {
            mNoteFactory.CreateNote(NotesRootDatabase.NoteSubjectFullPath, taskSubject, content);
            mLogger.LogDebug($"Note {taskSubject} created in {NotesRootDatabase.NoteSubjectFullPath}");
        }

        public IEnumerable<INote> GetAllNotes()
        {
            foreach (string filePath in
                Directory.EnumerateFiles(NotesRootDatabase.NoteSubjectFullPath, "*", SearchOption.AllDirectories))
            {
                yield return mNoteFactory.LoadNote(filePath);
            }
        }

        public string GetDatabasePath()
        {
            return mTasksDatabase.DatabaseDirectoryPath;
        }
    }
}