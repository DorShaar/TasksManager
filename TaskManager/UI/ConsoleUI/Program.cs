using CommandLine;
using Composition;
using ConsoleUI.Options;
using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Contracts;
using TaskManager.Contracts;
using UI.ConsolePrinter;

namespace ConsoleUI
{
    public class Program
    {
        private const int DESCRIPTION_LENGTH_LIMIT = 70;
        private static ILogger mLogger;
        private static ITaskManager mTaskManager;
        private static ConsolePrinter mConsolePrinter;

        public static void Main(string[] args)
        {
            TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
            mLogger = serviceProvider.GetLoggerService();
            mConsolePrinter = serviceProvider.GetConsolePrinterService();
            mTaskManager = serviceProvider.GetTaskManagerService();

            int exitCode = 1;
            using (Parser parser = new Parser(config => config.HelpWriter = Console.Out))
            {
                if (args.Length == 0)
                {
                    parser.ParseArguments<CommandLineOptions>(new[] { "--help" });
                    return;
                }

                exitCode = ParseArgument(parser, args);
            }

            if (exitCode != 0)
                Console.WriteLine($"Finished executing with exit code: {exitCode}");
        }

        private static int ParseArgument(Parser parser, string[] args)
        {
            return parser.ParseArguments<
                CommandLineOptions.GetOptions,
                CommandLineOptions.CreateOptions,
                CommandLineOptions.RemoveOptions,
                CommandLineOptions.CloseOptions,
                CommandLineOptions.MoveTaskOptions,
                CommandLineOptions.ReOpenTaskOptions,
                CommandLineOptions.OnWorkTaskOptions,
                CommandLineOptions.GetInformationTaskOptions,
                CommandLineOptions.OpenNoteOptions>(args).MapResult(
                    (CommandLineOptions.GetOptions options) => GetObject(options),
                    (CommandLineOptions.CreateOptions options) => CreateObject(options),
                    (CommandLineOptions.RemoveOptions options) => RemoveObject(options),
                    (CommandLineOptions.CloseOptions options) => CloseTask(options),
                    (CommandLineOptions.MoveTaskOptions options) => MoveTask(options),
                    (CommandLineOptions.ReOpenTaskOptions options) => ReOpenTask(options),
                    (CommandLineOptions.OnWorkTaskOptions options) => MarkTaskAsOnWork(options),
                    (CommandLineOptions.GetInformationTaskOptions options) => GetTaskInformation(options),
                    (CommandLineOptions.OpenNoteOptions options) => OpenNote(options),
                    (parserErrors) => 1
                );
        }

        private static int GetObject(CommandLineOptions.GetOptions options)
        {
            if(options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group, note, general, db)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return GetAllTasks(
                        options.ObjectName, options.Status, options.ShouldPrintAll, options.ShouldPrintNotOnlyDefault, options.Days, options.IsDetailed);

                case "group":
                case "groups":
                    return GatAllTaskGroup(options.ShouldPrintAll, options.IsDetailed);

                case "note":
                case "notes":
                    return GetNoteContent(options.ObjectName, options.ShouldPrintAll);

                case "general":
                    return GetGeneralNoteContent(options.ObjectName);

                case "db":
                case "data-base":
                case "database":
                    return GetDatabasePath();

                default:
                    mLogger.LogError("No valid object type given (task, group, note, general, db)");
                    return 1;
            }
        }

        /// <summary>
        /// Get all un-closed tasks.
        /// In case user choose to print all option, all tasks will be printed.
        /// </summary>
        private static int GetAllTasks(
            string taskGroup, string status, bool shouldPrintAll, bool shouldPrintNotOnlyDefaultGroup, int days, bool isDetailed)
        {
            IEnumerable<ITask> tasksToPrint = GetAllTasksOrTasksByGroupName(taskGroup);

            if (tasksToPrint == null)
            {
                mLogger.LogError($"No task group {taskGroup} exist");
                return 1;
            }

            if (!shouldPrintAll)
                tasksToPrint = tasksToPrint.Where(task => task.IsFinished == false);

            if (!shouldPrintNotOnlyDefaultGroup)
                tasksToPrint = tasksToPrint.Where(task => AreNamesEquals(task.Group, mTaskManager.DefaultTaskGroupName));

            if (!string.IsNullOrEmpty(status))
                tasksToPrint = tasksToPrint.Where(task => AreNamesEquals(task.Status.ToString(), status));

            if (days != 0)
                tasksToPrint = tasksToPrint.Where(task => IsTaskUpdateSince(task, days));

            mConsolePrinter.PrintTasks(tasksToPrint, isDetailed);
            return 0;
        }

        private static bool AreNamesEquals(string groupName1, string groupName2)
        {
            if (string.IsNullOrEmpty(groupName1) || string.IsNullOrEmpty(groupName2))
                return false;
            else
                return groupName1.Equals(groupName2, StringComparison.CurrentCultureIgnoreCase);
        }

        private static IEnumerable<ITask> GetAllTasksOrTasksByGroupName(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                return mTaskManager.GetAllTasks();
            else
                return GetTasksByGroupName(taskGroup);
        }

        private static IEnumerable<ITask> GetTasksByGroupName(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                return null;

            IEnumerable<ITask> tasks = mTaskManager.GetAllTasks((ITaskGroup task) => task.ID == taskGroup);
            if (tasks == null)
                tasks = mTaskManager.GetAllTasks((ITaskGroup task) => task.GroupName == taskGroup);

            return tasks;
        }

        private static bool IsTaskUpdateSince(ITask task, int days)
        {
            return task.TaskStatusHistory.TimeCreated.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeClosed.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeLastOnWork.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeLastOpened.AddDays(days) >= DateTime.Now;
        }

        private static int GatAllTaskGroup(bool shouldPrintAll, bool isDetailed)
        {
            IEnumerable<ITaskGroup> groupsToPrint = mTaskManager.GetAllTasksGroups();
            if (!shouldPrintAll)
                groupsToPrint = groupsToPrint.Where((ITaskGroup group) => (!group.IsFinished));

            mConsolePrinter.PrintTasksGroup(groupsToPrint, isDetailed);
            return 0;
        }

        private static int GetNoteContent(string notePath, bool shouldPrintAll)
        {
            if (shouldPrintAll)
                return GetAllNotesNames();

            INote note = GetNote(mTaskManager.NotesTasksDatabase, notePath);
            if (note != null)
                mLogger.Log(note.Text);

            return 0;
        }

        private static int GetGeneralNoteContent(string notePath)
        {
            INote note = GetNote(mTaskManager.NotesRootDatabase, notePath);
            if (note != null)
                mLogger.Log(note.Text);

            return 0;
        }

        private static int GetAllNotesNames()
        {
            IEnumerable<INote> allNotes = mTaskManager.GetAllNotes();
            IEnumerable<string> notesToPrint = allNotes
                .Where(note => Path.GetExtension(note.NotePath).Equals(note.Extension))
                .Select(note => Path.GetFileNameWithoutExtension(note.NotePath));

            mConsolePrinter.Print(notesToPrint, "NOTES");
            return 0;
        }

        private static int GetDatabasePath()
        {
            mConsolePrinter.Print(mTaskManager.GetDatabasePath(), "Database path");
            return 0;
        }

        private static int CreateObject(CommandLineOptions.CreateOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group, note, general)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return CreateNewTask(options.ObjectName, options.Description);

                case "group":
                case "groups":
                    return CreateNewTaskGroup(options.ObjectName);

                case "note":
                case "notes":
                    return CreateNote(options.ObjectName, options.Description);

                case "general note":
                case "general":
                    return CreateGeneralNote(options.ObjectName, options.Description);

                default:
                    mLogger.LogError("No valid object type given (task, group, note, general)");
                    return 1;
            }
        }

        private static int CreateNewTask(string taskGroupName, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                mLogger.LogError($"Cannot create empty task. Use -d for adding a description");
                return -1;
            }

            if(description.Length > 70)
            {
                mLogger.LogError($"Description too long. Description limitation is {DESCRIPTION_LENGTH_LIMIT} characters.");
                return -1;
            }

            if (!string.IsNullOrEmpty(taskGroupName))
            {
                ITaskGroup taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.ID == taskGroupName).FirstOrDefault();
                if (taskGroup == null)
                    taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.GroupName == taskGroupName).FirstOrDefault();

                if (taskGroup == null)
                {
                    mLogger.LogError($"Task group {taskGroupName} does not exist");
                    return 1;
                }

                mTaskManager.CreateNewTask(taskGroup, description);
            }
            else
                mTaskManager.CreateNewTask(description);

            return 0;
        }

        private static int CreateNewTaskGroup(string taskGroupName)
        {
            if (string.IsNullOrEmpty(taskGroupName))
            {
                mLogger.LogError($"{nameof(taskGroupName)} is null or empty");
                return 1;
            }

            mTaskManager.CreateNewTaskGroup(taskGroupName);
            return 0;
        }

        private static int CreateNote(string taskId, string textToWrite)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError($"No task id given to create note");
                return 1;
            }

            if (textToWrite == null)
                textToWrite = string.Empty;

            mTaskManager.CreateTaskNote(taskId, textToWrite);
            return 0;
        }

        private static int CreateGeneralNote(string noteSubject, string textToWrite)
        {
            if (string.IsNullOrEmpty(noteSubject))
            {
                mLogger.LogError($"No task subject given to create note");
                return 1;
            }

            if (textToWrite == null)
                textToWrite = string.Empty;

            mTaskManager.CreateGeneralNote(noteSubject, textToWrite);
            return 0;
        }

        private static int RemoveObject(CommandLineOptions.RemoveOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return RemoveTask(options.ObjectId);

                case "group":
                case "groups":
                    return RemoveTaskGroup(options.ObjectId, options.ShouldHardDelete);

                default:
                    mLogger.LogError("No valid object type given (task, group)");
                    return 1;
            }
        }

        private static int RemoveTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError($"No task id given to remove");
                return 1;
            }

            mTaskManager.RemoveTask(taskId);
            return 0;
        }

        private static int RemoveTaskGroup(string taskGroup, bool shouldHardDelete)
        {
            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError($"No group name or group id given");
                return 1;
            }

            if (shouldHardDelete)
            {
                mLogger.Log("Are you sure you want to delete that group with all of its inner tasks? If so, press y");
                string userInput = Console.ReadLine();
                if (userInput.ToLower() != "y")
                {
                    mLogger.Log($"Group {taskGroup} was not deleted");
                    return 0;
                }
            }

            mTaskManager.RemoveTaskGroup(taskGroup, !shouldHardDelete);
            return 0;
        }

        private static int CloseTask(CommandLineOptions.CloseOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return CloseTask(options.ObjectId, options.Reason);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private static int CloseTask(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            mTaskManager.CloseTask(taskId, reason ?? string.Empty);
            return 0;
        }

        private static int OpenNote(CommandLineOptions.OpenNoteOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (note, general)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "note":
                case "notes":
                    return OpenNote(options.NoteName);

                case "general":
                    return OpenGeneralNote(options.NoteName);

                default:
                    mLogger.LogError("No valid object type given (note, general)");
                    return 1;
            }
        }

        private static int OpenNote(string noteName)
        {
            INote note = GetNote(mTaskManager.NotesTasksDatabase, noteName);
            if (note != null)
                note.Open();

            return 0;
        }

        private static int OpenGeneralNote(string noteName)
        {
            INote note = GetNote(mTaskManager.NotesRootDatabase, noteName);
            if (note != null)
                note.Open();

            return 0;
        }

        private static INote GetNote(INotesSubject notesSubject, string notePath)
        {
            NotesDirectoryIterator directoryIterator = new NotesDirectoryIterator(notesSubject, mConsolePrinter);
            return directoryIterator.GetNote(notePath);
        }

        private static int MoveTask(CommandLineOptions.MoveTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to move");
                return 1;
            }

            if (!string.IsNullOrEmpty(options.TaskGroup))
                mTaskManager.MoveTaskToGroup(options.TaskId, options.TaskGroup);
            else
            {
                mLogger.LogError($"No group name or group id given");
                return 1;
            }

            return 0;
        }

        private static int ReOpenTask(CommandLineOptions.ReOpenTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            mTaskManager.ReOpenTask(options.TaskId, options.Reason ?? string.Empty);
            return 0;
        }

        private static int MarkTaskAsOnWork(CommandLineOptions.OnWorkTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            mTaskManager.MarkTaskOnWork(options.TaskId, options.Reason ?? string.Empty);
            return 0;
        }

        private static int GetTaskInformation(CommandLineOptions.GetInformationTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to create note");
                return 1;
            }

            mConsolePrinter.PrintTaskInformation(
                 mTaskManager.GetAllTasks((ITask task) => task.ID == options.TaskId).First());
            return 0;
        }
    }
}