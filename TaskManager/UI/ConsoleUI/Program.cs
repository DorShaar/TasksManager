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

namespace ConsoleUI
{
    public class Program
    {
        private static ILogger mLogger;
        private static readonly ConsolePrinter mConsolePrinter = new ConsolePrinter();

        public static void Main(string[] args)
        {
            TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
            mLogger = serviceProvider.GetLoggerService();
            ITaskManager taskManager = serviceProvider.GetTaskManagerService();

            var parser = new Parser(config => config.HelpWriter = Console.Out);
            if (args.Length == 0)
            {
                parser.ParseArguments<TaskOptions>(new[] { "--help" });
                return;
            }

            int exitCode = parser.ParseArguments<
            TaskGroupOptions.CreateNewTaskGroupOptions,
            TaskGroupOptions.GatAllTaskGroupOptions,
            TaskGroupOptions.RemoveTaskGroupOptions,

            TaskOptions.CreateNewTaskOptions,
            TaskOptions.GetAllTasksOptions,
            TaskOptions.CloseTasksOptions,
            TaskOptions.RemoveTaskOptions,
            TaskOptions.MoveTaskOptions,
            TaskOptions.ReOpenTaskOptions,
            TaskOptions.OnWorkTaskOptions,
            TaskOptions.GetInformationTaskOptions,

            NotesOptions.CreateNoteOptions,
            NotesOptions.CreateGeneralNoteOptions,
            NotesOptions.GetNotesOptions,
            NotesOptions.OpenNoteOptions,
            //NotesOptions.GetNoteOptions,

            ConfigOptions.GetDatabasePathOptions
            /*ConfigOptions.SetDatabasePathOptions*/>(args).MapResult(
                 (TaskGroupOptions.CreateNewTaskGroupOptions options) => CreateNewTaskGroup(taskManager, options),
                 (TaskGroupOptions.GatAllTaskGroupOptions options) => GatAllTaskGroup(taskManager, options),
                 (TaskGroupOptions.RemoveTaskGroupOptions options) => RemoveTaskGroup(taskManager, options),

                 (TaskOptions.CreateNewTaskOptions options) => CreateNewTask(taskManager, options),
                 (TaskOptions.GetAllTasksOptions options) => GetAllTasks(taskManager, options),
                 (TaskOptions.CloseTasksOptions options) => CloseTask(taskManager, options),
                 (TaskOptions.RemoveTaskOptions options) => RemoveTaskOptions(taskManager, options),
                 (TaskOptions.MoveTaskOptions options) => MoveTask(taskManager, options),
                 (TaskOptions.ReOpenTaskOptions options) => ReOpenTask(taskManager, options),
                 (TaskOptions.OnWorkTaskOptions options) => MarkTaskAsOnWork(taskManager, options),
                 (TaskOptions.GetInformationTaskOptions options) => GetTaskInformation(taskManager, options),

                 (NotesOptions.CreateNoteOptions options) => CreateNote(taskManager, options),
                 (NotesOptions.CreateGeneralNoteOptions options) => CreateGeneralNote(taskManager, options),
                 (NotesOptions.GetNotesOptions options) => GetNotes(taskManager, options),
                 (NotesOptions.OpenNoteOptions options) => OpenNote(taskManager, options),
                 //(NotesOptions.GetNoteOptions options) => GetNote(taskManager, options),

                 //(ConfigOptions.SetDatabasePathOptions options) => SetDatabasePath(taskManager, options),
                 (ConfigOptions.GetDatabasePathOptions options) => GetDatabasePath(taskManager, options),
                 (parserErrors) => 1
            );

            if (exitCode != 0)
                Console.WriteLine($"Finished executing with exit code: {exitCode}");
        }

        private static int CreateNewTaskGroup(ITaskManager taskManager, TaskGroupOptions.CreateNewTaskGroupOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskGroupName))
            {
                mLogger.LogError($"{nameof(options.TaskGroupName)} is null or empty");
                return 1;
            }

            taskManager.CreateNewTaskGroup(options.TaskGroupName);
            return 0;
        }

        private static int CreateNewTask(ITaskManager taskManager, TaskOptions.CreateNewTaskOptions options)
        {
            if (!string.IsNullOrEmpty(options.TaskGroup))
            {
                ITaskGroup taskGroup = taskManager.GetAllTasksGroups().Where(group => group.ID == options.TaskGroup).FirstOrDefault();
                if (taskGroup == null)
                    taskGroup = taskManager.GetAllTasksGroups().Where(group => group.GroupName == options.TaskGroup).FirstOrDefault();

                if (taskGroup == null)
                {
                    mLogger.LogError($"Task group {options.TaskGroup} does not exist");
                    return 1;
                }

                taskManager.CreateNewTask(taskGroup, options.Description);
            }
            else
                taskManager.CreateNewTask(options.Description);

            return 0;
        }

        private static int GatAllTaskGroup(ITaskManager taskManager, TaskGroupOptions.GatAllTaskGroupOptions options)
        {
            IEnumerable<ITaskGroup> groupsToPrint = taskManager.GetAllTasksGroups();
            if (!options.ShouldPrintAll)
                groupsToPrint = groupsToPrint.Where((ITaskGroup group) => (!group.IsFinished));

            mConsolePrinter.PrintTasksGroup(groupsToPrint, options);
            return 0;
        }

        /// <summary>
        /// Get all un-closed tasks.
        /// In case user chose print all option, all tasks will be printed.
        /// </summary>
        private static int GetAllTasks(ITaskManager taskManager, TaskOptions.GetAllTasksOptions options)
        {
            IEnumerable<ITask> allTasks;

            if (!string.IsNullOrEmpty(options.TaskGroup))
            {
                allTasks = taskManager.GetAllTasks((ITaskGroup task) => task.ID == options.TaskGroup);
                if (allTasks == null)
                    allTasks = taskManager.GetAllTasks((ITaskGroup task) => task.GroupName == options.TaskGroup);

                if(allTasks == null)
                {
                    mLogger.LogError($"No task group {options.TaskGroup} exist");
                    return 1;
                }
            }
            else
                allTasks = taskManager.GetAllTasks();

            IEnumerable<ITask> tasksToPrint = allTasks;

            if (!string.IsNullOrEmpty(options.Status) && options.ShouldPrintAll)
            {
                mLogger.LogError("Cannot quary all tasks with specific status quary");
                return 1;
            }

            if(!string.IsNullOrEmpty(options.Status))
                tasksToPrint = tasksToPrint.Where(task => task.Status.ToString().Equals(options.Status, StringComparison.CurrentCultureIgnoreCase));
            else if (!options.ShouldPrintAll)
                tasksToPrint = tasksToPrint.Where(task => task.IsFinished == false);

            if (options.Hours != 0)
                tasksToPrint = tasksToPrint.Where(
                     task => task.TimeCreated.AddHours(options.Hours) >= DateTime.Now);

            if (options.Days != 0)
                tasksToPrint = tasksToPrint.Where(
                     task => task.TimeCreated.AddDays(options.Days) >= DateTime.Now);

            mConsolePrinter.PrintTasks(tasksToPrint, options);
            return 0;
        }

        private static int CloseTask(ITaskManager taskManager, TaskOptions.CloseTasksOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            taskManager.CloseTask(options.TaskId);
            return 0;
        }

        private static int ReOpenTask(ITaskManager taskManager, TaskOptions.ReOpenTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            taskManager.ReOpenTask(options.TaskId);
            return 0;
        }

        private static int MarkTaskAsOnWork(ITaskManager taskManager, TaskOptions.OnWorkTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            taskManager.MarkTaskOnWork(options.TaskId);
            return 0;
        }

        private static int GetTaskInformation(ITaskManager taskManager, TaskOptions.GetInformationTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to create note");
                return 1;
            }

            mConsolePrinter.PrintTaskInformation(
                 taskManager.GetAllTasks((ITask task) => task.ID == options.TaskId).First());
            return 0;
        }

        private static int RemoveTaskGroup(ITaskManager taskManager, TaskGroupOptions.RemoveTaskGroupOptions options)
        {
            if (options.ShouldHardDelete)
            {
                mLogger.Log("Would you like to delete that group with all of its inner tasks? If so, press y");
                string userInput = Console.ReadLine();
                if (userInput.ToLower() != "y")
                    return 0;
            }

            if (!string.IsNullOrEmpty(options.TaskGroup))
                taskManager.RemoveTaskGroup(options.TaskGroup, !options.ShouldHardDelete);
            else
            {
                mLogger.LogError($"No group name or group id given");
                return 1;
            }

            return 0;
        }

        private static int RemoveTaskOptions(ITaskManager taskManager, TaskOptions.RemoveTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to remove");
                return 1;
            }

            taskManager.RemoveTask(options.TaskId);
            return 0;
        }

        private static int MoveTask(ITaskManager taskManager, TaskOptions.MoveTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to move");
                return 1;
            }

            if (!string.IsNullOrEmpty(options.TaskGroup))
                taskManager.MoveTaskToGroup(options.TaskId, options.TaskGroup);
            else
            {
                mLogger.LogError($"No group name or group id given");
                return 1;
            }

            return 0;
        }

        private static int CreateNote(ITaskManager taskManager, NotesOptions.CreateNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to create note");
                return 1;
            }

            string textToWrite = options.Text;
            if (options.Text == null)
                textToWrite = string.Empty;

            taskManager.CreateNote(options.TaskId, textToWrite);
            return 0;
        }

        private static int CreateGeneralNote(ITaskManager taskManager, NotesOptions.CreateGeneralNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.NoteSubject))
            {
                mLogger.LogError($"No task subject given to create note");
                return 1;
            }

            if (options.NoteSubject.Length > 20)
            {
                mLogger.LogError($"Note subject {options.NoteSubject} is too long. Must be 20 characters");
                return 1;
            }

            string textToWrite = options.Text;
            if (options.Text == null)
                textToWrite = string.Empty;

            taskManager.CreateGeneralNote(options.NoteSubject, textToWrite);
            return 0;
        }

        private static int GetNotes(ITaskManager taskManager, NotesOptions.GetNotesOptions options)
        {
            IEnumerable<INote> allNotes = taskManager.GetNotes();
            IEnumerable<string> notesToPrint = allNotes.Select(note => Path.GetFileNameWithoutExtension(note.NotePath));

            // Print only general notes.
            if (!options.ShouldPrintAllNotes)
                notesToPrint = notesToPrint.Where(note => !int.TryParse(note, out int _));

            mLogger.Log("NOTES");
            foreach(string noteName in notesToPrint)
            {
                mLogger.Log(noteName);
            }

            return 0;
        }

        private static int OpenNote(ITaskManager taskManager, NotesOptions.OpenNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.NoteName))
            {
                mLogger.LogError($"No task id given to open note");
                return 1;
            }

            taskManager.OpenNote(options.NoteName);
            return 0;
        }

        private static int GetNote(ITaskManager taskManager, NotesOptions.GetNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.NoteName))
            {
                mLogger.LogError($"No task id given to get note");
                return 1;
            }

            mLogger.Log(taskManager.GetNote(options.NoteName));
            return 0;
        }

        private static int GetDatabasePath(ITaskManager taskManager, ConfigOptions.GetDatabasePathOptions _)
        {
            mConsolePrinter.Print(taskManager.GetDatabasePath(), "Database path");
            return 0;
        }

        private static int SetDatabasePath(ITaskManager taskManager, ConfigOptions.SetDatabasePathOptions options)
        {
            taskManager.ChangeDatabasePath(options.NewDatabasePath);
            return 0;
        }
    }
}