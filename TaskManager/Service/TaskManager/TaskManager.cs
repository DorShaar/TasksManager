using Database.Contracts;
using Logger.Contracts;
using System;
using System.Collections.Generic;
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

        private readonly ITaskGroupBuilder mTaskGroupBuilder;
        private readonly IRepository<ITaskGroup> mDatabase;

        private readonly INoteBuilder mNoteBuilder;
        private readonly GeneralNotesDatabase mNotesDatabase;

        public TaskManager(IRepository<ITaskGroup> database, ITaskGroupBuilder taskGroupBuilder, INoteBuilder noteBuilder, ILogger logger)
        {
            mLogger = logger;

            mDatabase = database;
            mTaskGroupBuilder = taskGroupBuilder;

            mNotesDatabase = new GeneralNotesDatabase(mDatabase.NotesDatabaseDirectoryPath, noteBuilder, mLogger);
            mNoteBuilder = noteBuilder;

            InitializeFreeTasksGroup();
        }

        private void InitializeFreeTasksGroup()
        {
            mFreeTasksGroup = mDatabase.GetEntity(FreeTaskGroupName);

            if (mFreeTasksGroup == null)
            {
                mFreeTasksGroup = mTaskGroupBuilder.Create(FreeTaskGroupName, mLogger);
                mDatabase.Insert(mFreeTasksGroup);
            }
        }

        /// <summary>
        /// Create new task group.
        /// </summary>
        public void CreateNewTaskGroup(string groupName)
        {
            mDatabase.Insert(mTaskGroupBuilder.Create(groupName, mLogger));
        }

        public void RemoveTaskGroup(string tasksGroup, bool shouldMoveInnerTasks)
        {
            ITaskGroup taskGroup = mDatabase.GetEntity(tasksGroup);
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

            mDatabase.Remove(taskGroup);
        }

        public IEnumerable<ITaskGroup> GetAllTasksGroups()
        {
            return mDatabase.GetAll();
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
            mDatabase.AddOrUpdate(tasksGroup);
            return task;
        }

        /// <summary>
        /// Create new task into <see cref="mFreeTasksGroup"/>.
        /// </summary>
        public void CreateNewTask(string description)
        {
            mFreeTasksGroup.CreateTask(description);
            mDatabase.Update(mFreeTasksGroup);
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
                {
                    yield return task;
                }
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

        public void CloseTask(string taskId)
        {
            foreach (ITaskGroup group in mDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    task.CloseTask();
                    mDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void ReOpenTask(string taskId)
        {
            foreach (ITaskGroup group in mDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    task.CloseTask();
                    mDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void RemoveTask(string taskId)
        {
            foreach (ITaskGroup group in mDatabase.GetAll())
            {
                ITask task = group.GetTask(taskId);
                if (task != null)
                {
                    group.RemoveTask(taskId);
                    mDatabase.Update(group);
                    return;
                }
            }

            mLogger.LogError($"Task {taskId} was not found");
        }

        public void MoveTaskToGroup(string taskId, string taskGroup)
        {
            ITaskGroup taskGroupDestination = mDatabase.GetEntity(taskGroup);
            if (taskGroup == null)
            {
                mLogger.LogError($"Task group {taskGroup} was not found");
                return;
            }

            MoveTaskToGroup(taskId, taskGroupDestination);
        }

        private void MoveTaskToGroup(string taskId, ITaskGroup taskGroupDestination)
        {
            foreach (ITaskGroup sourceGroup in mDatabase.GetAll())
            {
                ITask task = sourceGroup.GetTask(taskId);
                if (task != null)
                {
                    taskGroupDestination.AddTask(task);
                    mDatabase.Update(taskGroupDestination);

                    sourceGroup.RemoveTask(taskId);
                    mDatabase.Update(sourceGroup);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateNote(string taskId, string content)
        {
            foreach (ITaskGroup taskGroup in mDatabase.GetAll())
            {
                ITask task = taskGroup.GetTask(taskId);
                if (task != null)
                {
                    task.CreateNote(mDatabase.NotesDatabaseDirectoryPath, content);
                    mDatabase.Update(taskGroup);
                    return;
                }
            }

            mLogger.LogError($"Task id {taskId} was not found");
        }

        public void CreateGeneralNote(string taskSubject, string content)
        {
            mNoteBuilder.CreateNote(mDatabase.NotesDatabaseDirectoryPath, taskSubject, content);
            mLogger.Log($"Note {taskSubject} created in {mDatabase.NotesDatabaseDirectoryPath}");
        }

        public void OpenNote(string noteName)
        {
            // First searches on general notes.
            INote note = mNotesDatabase.GetNote(noteName);
            if(note != null)
            {
                note.Open();
                return;
            }

            // Secondly searches on private notes (in tasks).
            string taskId = noteName;
            foreach (ITaskGroup taskGroup in mDatabase.GetAll())
            {
                ITask task = taskGroup.GetTask(taskId);
                if (task != null)
                {
                    task.OpenNote();
                    return;
                }
            }

            mLogger.LogError($"Note {noteName} was not found");
        }

        public string GetNote(string noteName)
        {
            // First searches on general notes.
            INote note = mNotesDatabase.GetNote(noteName);
            if (note != null)
                return note.Text;

            // Secondly searches on private notes (in tasks).
            string taskId = noteName;
            foreach (ITaskGroup taskGroup in mDatabase.GetAll())
            {
                ITask task = taskGroup.GetTask(taskId);
                if (task != null)
                    return task.GetNote();
            }

            mLogger.LogError($"Note {noteName} was not found");
            return string.Empty;
        }

        public string GetDatabasePath()
        {
            return mDatabase.DatabasePath;
        }

        public void ChangeDatabasePath(string newDatabasePath)
        {
            mDatabase.SetDatabasePath(newDatabasePath);
        }
    }
}