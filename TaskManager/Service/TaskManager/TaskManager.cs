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

        private readonly INoteBuilder mNoteBuilder;
        private readonly INotesSubjectBuilder mNoteSubjectBuilder;
        private readonly ITasksGroupBuilder mTaskGroupBuilder;

        private readonly ILocalRepository<ITasksGroup> mTasksDatabase;

        internal static readonly string FreeTaskGroupName = "Free";

        public INotesSubject NotesRootDatabase { get; }
        public INotesSubject NotesTasksDatabase { get; }
        public ITasksGroup FreeTasksGroup { get; private set; }
        public ITasksGroup DefaultTaskGroupName { get; }

        public TaskManager(
            ILocalRepository<ITasksGroup> tasksDatabase,
            ITasksGroupBuilder taskGroupBuilder,
            INoteBuilder noteBuilder,
            INotesSubjectBuilder notesSubjectBuilder,
            ILogger logger)
        {
            mLogger = logger;

            mTaskGroupBuilder = taskGroupBuilder;
            mNoteBuilder = noteBuilder;
            mNoteSubjectBuilder = notesSubjectBuilder;

            mTasksDatabase = tasksDatabase;
            NotesRootDatabase = mNoteSubjectBuilder.Load(mTasksDatabase.NotesDirectoryPath);
            NotesTasksDatabase = mNoteSubjectBuilder.Load(mTasksDatabase.NotesTasksDirectoryPath);
            DefaultTaskGroupName = mTasksDatabase.GetEntity(mTasksDatabase.DefaultTasksGroup);

            InitializeFreeTasksGroup();
        }

        private void InitializeFreeTasksGroup()
        {
            FreeTasksGroup = mTasksDatabase.GetEntity(FreeTaskGroupName);

            if (FreeTasksGroup == null)
            {
                FreeTasksGroup = mTaskGroupBuilder.Create(FreeTaskGroupName, mLogger);
                mTasksDatabase.Insert(FreeTasksGroup);
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
                mLogger.LogError($"Given task group is null");
                return null;
            }

            IWorkTask task = tasksGroup.CreateTask(description);
            mTasksDatabase.AddOrUpdate(tasksGroup);
            return task;
        }

        /// <summary>
        /// Create new task into <see cref="FreeTasksGroup"/>.
        /// </summary>
        public void CreateNewTask(string description)
        {
            FreeTasksGroup.CreateTask(description);
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
                IWorkTask task = group.GetTask(taskId);
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
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId);
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
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId);
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
            foreach (ITasksGroup group in mTasksDatabase.GetAll())
            {
                IWorkTask task = group.GetTask(taskId);
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
                mLogger.LogError($"Destination task group was not found");
                return;
            }

            foreach (ITasksGroup sourceGroup in mTasksDatabase.GetAll())
            {
                IWorkTask task = sourceGroup.GetTask(taskId);
                if (task != null)
                {
                    ITasksGroup sourceTaskGroup = mTasksDatabase.GetEntity(task.GroupName);

                    if (sourceTaskGroup == null)
                    {
                        mLogger.LogError($"Moving task failed since source group is not given, unable checking moving to same group");
                        return;
                    }

                    if (sourceTaskGroup != destionationTaskGroup)
                    {
                        destionationTaskGroup.AddTask(task);
                        mTasksDatabase.Update(destionationTaskGroup);

                        sourceGroup.RemoveTask(taskId);
                        mTasksDatabase.Update(sourceGroup);
                        return;
                    }
                    else
                    {
                        mLogger.LogError($"Moving task failed since Source group and destination group are the same");
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
                IWorkTask task = taskGroup.GetTask(taskId);
                if (task != null)
                {
                    task.CreateNote(NotesTasksDatabase.NoteSubjectFullPath, content);
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
            return mTasksDatabase.DatabaseDirectoryPath;
        }
    }
}