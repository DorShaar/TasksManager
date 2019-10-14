using Database.Contracts;
using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.Contracts;
using TaskManager.Contracts;

[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
namespace TaskManager
{
    public class TaskManager : ITaskManager
    {
        private readonly ILogger mLogger;

        private ITaskGroup mFreeTasksGroup;
        internal static readonly string FreeTaskGroupName = "Free";

        private readonly INoteBuilder mNoteBuilder;
        private readonly INotesSubjectBuilder mNoteSubjectBuilder;
        private readonly ITaskGroupBuilder mTaskGroupBuilder;

        private readonly ILocalRepository<ITaskGroup> mTasksDatabase;
        public INotesSubject NotesRootDatabase { get; }
        public INotesSubject NotesTasksDatabase { get; }

        public TaskManager(
            ILocalRepository<ITaskGroup> tasksDatabase,
            ITaskGroupBuilder taskGroupBuilder,
            INoteBuilder noteBuilder,
            INotesSubjectBuilder notesSubjectBuilder,
            ILogger logger)
        {
            mLogger = logger;

            mTaskGroupBuilder = taskGroupBuilder;
            mNoteBuilder = noteBuilder;
            mNoteSubjectBuilder = notesSubjectBuilder;

            mTasksDatabase = tasksDatabase;
            NotesRootDatabase = mNoteSubjectBuilder.Load(mTasksDatabase.NotesDatabaseDirectoryPath);
            NotesTasksDatabase = mNoteSubjectBuilder.Load(mTasksDatabase.NotesTasksDatabaseDirectoryPath);

            InitializeFreeTasksGroup();
        }

        private void InitializeFreeTasksGroup()
        {
            mFreeTasksGroup = mTasksDatabase.GetEntity(FreeTaskGroupName);

            if (mFreeTasksGroup == null)
            {
                mFreeTasksGroup = mTaskGroupBuilder.Create(FreeTaskGroupName, mLogger);
                mTasksDatabase.Insert(mFreeTasksGroup);
            }
        }

        /// <summary>
        /// Create new task group.
        /// </summary>
        public void CreateNewTaskGroup(string groupName)
        {
            mTasksDatabase.Insert(mTaskGroupBuilder.Create(groupName, mLogger));
        }

        public void RemoveTaskGroup(string tasksGroup, bool shouldMoveInnerTasks)
        {
            ITaskGroup taskGroup = mTasksDatabase.GetEntity(tasksGroup);
            if (taskGroup == null)
            {
                mLogger.LogError($"Task group {tasksGroup} does not exists");
                return;
            }

            if (taskGroup == mFreeTasksGroup)
            {
                mLogger.LogError($"Cannot delete {FreeTaskGroupName} from database");
                return;
            }

            if (shouldMoveInnerTasks)
            {
                // Should iterate with for and not for each because we are changing the size of the IEnumarable.
                ITask[] tasksToMove = taskGroup.GetAllTasks().ToArray();
                for (int i = 0; i < tasksToMove.Length; ++i)
                {
                    MoveTaskToGroup(tasksToMove[i].ID, mFreeTasksGroup);
                }
            }

            mTasksDatabase.Remove(taskGroup);
        }

        public IEnumerable<ITaskGroup> GetAllTasksGroups()
        {
            return mTasksDatabase.GetAll();
        }

        /// <summary>
        /// Create new task into <param name="tasksGroup"/>.
        /// </summary>
        public ITask CreateNewTask(ITaskGroup tasksGroup, string description)
        {
            if (tasksGroup == null)
            {
                mLogger.LogError($"Given task group is null");
                return null;
            }

            ITask task = tasksGroup.CreateTask(description);
            mTasksDatabase.AddOrUpdate(tasksGroup);
            return task;
        }

        /// <summary>
        /// Create new task into <see cref="mFreeTasksGroup"/>.
        /// </summary>
        public void CreateNewTask(string description)
        {
            mFreeTasksGroup.CreateTask(description);
            mTasksDatabase.Update(mFreeTasksGroup);
        }

        public IEnumerable<ITask> GetAllTasks()
        {
            IEnumerable<ITask> allTasks = new List<ITask>();

            foreach (ITaskGroup taskGroup in GetAllTasksGroups())
            {
                allTasks = allTasks.Concat(taskGroup.GetAllTasks());
            }

            return allTasks;
        }

        public IEnumerable<ITask> GetAllTasks(Func<ITask, bool> action)
        {
            foreach (ITask task in GetAllTasks())
            {
                if (action(task))
                    yield return task;
            }
        }

        public IEnumerable<ITask> GetAllTasks(Func<ITaskGroup, bool> action)
        {
            foreach (ITaskGroup taskGroup in GetAllTasksGroups())
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
            foreach (ITaskGroup group in mTasksDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    task.CloseTask(reason);
                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void ReOpenTask(string taskId, string reason)
        {
            foreach (ITaskGroup group in mTasksDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    task.ReOpenTask(reason);
                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void MarkTaskOnWork(string taskId, string reason)
        {
            foreach (ITaskGroup group in mTasksDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    task.MarkTaskOnWork(reason);
                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void RemoveTask(string taskId)
        {
            foreach (ITaskGroup group in mTasksDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    group.RemoveTask(taskId);
                    mTasksDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task {taskId} was not found");
        }

        public void MoveTaskToGroup(string taskId, string taskGroup)
        {
            ITaskGroup taskGroupDestination = mTasksDatabase.GetEntity(taskGroup);
            if (taskGroup == null)
            {
                mLogger.LogError($"Task group {taskGroup} was not found");
                return;
            }

            MoveTaskToGroup(taskId, taskGroupDestination);
        }

        private void MoveTaskToGroup(string taskId, ITaskGroup taskGroupDestination)
        {
            foreach (ITaskGroup sourceGroup in mTasksDatabase.GetAll())
            {
                ITask task = sourceGroup.GetTask(taskId);
                if (task != null)
                {
                    taskGroupDestination.AddTask(task);
                    mTasksDatabase.Update(taskGroupDestination);

                    sourceGroup.RemoveTask(taskId);
                    mTasksDatabase.Update(sourceGroup);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateTaskNote(string taskId, string content)
        {
            foreach (ITaskGroup taskGroup in mTasksDatabase.GetAll())
            {
                ITask task = taskGroup.GetTask(taskId);
                if (task != null)
                {
                    task.CreateNote(NotesRootDatabase.NoteSubjectFullPath, content);
                    mTasksDatabase.Update(taskGroup);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateGeneralNote(string taskSubject, string content)
        {
            mNoteBuilder.CreateNote(NotesRootDatabase.NoteSubjectFullPath, taskSubject, content);
            mLogger.Log($"Note {taskSubject} created in {NotesRootDatabase.NoteSubjectFullPath}");
        }

        public IEnumerable<INote> GetAllNotes()
        {
            foreach (string filePath in
                Directory.EnumerateFiles(NotesRootDatabase.NoteSubjectFullPath, "*", SearchOption.AllDirectories))
            {
                yield return mNoteBuilder.Load(filePath);
            }
        }

        public string GetDatabasePath()
        {
            return mTasksDatabase.DatabasePath;
        }

        public void ChangeDatabasePath(string newDatabasePath)
        {
            mTasksDatabase.SetDatabasePath(newDatabasePath);
        }
    }
}