﻿using CommandLine;
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
        private static ITaskManager mTaskManager;
        private static readonly ConsolePrinter mConsolePrinter = new ConsolePrinter();

        public static void Main(string[] args)
        {
            TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
            mLogger = serviceProvider.GetLoggerService();
            mTaskManager = serviceProvider.GetTaskManagerService();

            int exitCode = 1;
            using (Parser parser = new Parser(config => config.HelpWriter = Console.Out))
            {
                if (args.Length == 0)
                {
                    parser.ParseArguments<TaskOptions>(new[] { "--help" });
                    return;
                }

                exitCode = parser.ParseArguments<
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
                //NotesOptions.OpenNoteOptions,
                //NotesOptions.GetNoteOptions,

                ConfigOptions.GetDatabasePathOptions
                /*ConfigOptions.SetDatabasePathOptions*/>(args).MapResult(
                    (CommandLineOptions.GetOptions options) => GetObject(options),
                    (CommandLineOptions.CreateOptions options) => CreateObject(options),
                    (CommandLineOptions.RemoveOptions options) => RemoveObject(options),
                    (CommandLineOptions.CloseOptions options) => CloseTask(options),

                    (CommandLineOptions.OpenOptions options) => OpenNote(options),



                     (TaskOptions.MoveTaskOptions options) => MoveTask(mTaskManager, options),
                     (TaskOptions.ReOpenTaskOptions options) => ReOpenTask(mTaskManager, options),
                     (TaskOptions.OnWorkTaskOptions options) => MarkTaskAsOnWork(mTaskManager, options),
                     (TaskOptions.GetInformationTaskOptions options) => GetTaskInformation(mTaskManager, options),

                     //(ConfigOptions.SetDatabasePathOptions options) => SetDatabasePath(taskManager, options),
                     (parserErrors) => 1
                );
            }

            if (exitCode != 0)
                Console.WriteLine($"Finished executing with exit code: {exitCode}");
        }

        private static int GetObject(CommandLineOptions.GetOptions options)
        {
            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return GetAllTasks(
                        options.ObjectName, options.Status, options.ShouldPrintAll, options.Hours, options.Days, options.IsDetailed);

                case "group":
                case "groups":
                    return GatAllTaskGroup(options.ShouldPrintAll, options.IsDetailed);

                case "note":
                case "notes":
                    return GetNote(options.ObjectName);

                case "db":
                case "data-base":
                case "database":
                    return GetDatabasePath();

                default:
                    return 1;
            }
        }

        /// <summary>
        /// Get all un-closed tasks.
        /// In case user chose print all option, all tasks will be printed.
        /// </summary>
        private static int GetAllTasks(string taskGroup, string status, bool shouldPrintAll, int hours, int days, bool isDetailed)
        {
            IEnumerable<ITask> tasksToPrint = GetTasksToPrintByOptions(taskGroup);
            if (tasksToPrint == null)
                return 1;

            if (!string.IsNullOrEmpty(status) && shouldPrintAll)
            {
                mLogger.LogError("Cannot quary all tasks with specific status quary");
                return 1;
            }

            if (!string.IsNullOrEmpty(status))
                tasksToPrint = tasksToPrint.Where(task => task.Status.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase));
            else if (!shouldPrintAll)
                tasksToPrint = tasksToPrint.Where(task => task.IsFinished == false);

            if (hours != 0)
                tasksToPrint = tasksToPrint
                    .Where(task => task.TimeCreated.AddHours(hours) >= DateTime.Now ||
                                   task.TimeClosed.AddHours(hours) >= DateTime.Now ||
                                   task.TimeLastOnWork.AddHours(hours) >= DateTime.Now ||
                                   task.TimeLastOpened.AddHours(hours) >= DateTime.Now);

            if (days != 0)
                tasksToPrint = tasksToPrint
                    .Where(task => task.TimeCreated.AddDays(days) >= DateTime.Now ||
                                   task.TimeClosed.AddDays(days) >= DateTime.Now ||
                                   task.TimeLastOnWork.AddDays(days) >= DateTime.Now ||
                                   task.TimeLastOpened.AddDays(days) >= DateTime.Now);

            mConsolePrinter.PrintTasks(tasksToPrint, isDetailed);
            return 0;
        }

        private static IEnumerable<ITask> GetTasksToPrintByOptions(string taskGroup)
        {
            IEnumerable<ITask> allTasks = null;

            if (!string.IsNullOrEmpty(taskGroup))
            {
                allTasks = mTaskManager.GetAllTasks((ITaskGroup task) => task.ID == taskGroup);
                if (allTasks == null)
                    allTasks = mTaskManager.GetAllTasks((ITaskGroup task) => task.GroupName == taskGroup);

                if (allTasks == null)
                    mLogger.LogError($"No task group {taskGroup} exist");
            }
            else
                allTasks = mTaskManager.GetAllTasks();

            return allTasks;
        }

        private static int GatAllTaskGroup(bool shouldPrintAll, bool isDetailed)
        {
            IEnumerable<ITaskGroup> groupsToPrint = mTaskManager.GetAllTasksGroups();
            if (!shouldPrintAll)
                groupsToPrint = groupsToPrint.Where((ITaskGroup group) => (!group.IsFinished));

            mConsolePrinter.PrintTasksGroup(groupsToPrint, isDetailed);
            return 0;
        }

        private static int GetNote(string noteName)
        {
            // Prints all notes
            if (string.IsNullOrEmpty(noteName))
                return GetAllNotesNames();

            // Prints specific note.
            mLogger.Log(mTaskManager.GetNote(noteName));
            return 0;
        }

        private static int GetAllNotesNames()
        {
            IEnumerable<INote> allNotes = mTaskManager.GetNotes();
            IEnumerable<string> notesToPrint = allNotes.Select(note => Path.GetFileNameWithoutExtension(note.NotePath));

            mLogger.Log("NOTES");
            foreach (string noteName in notesToPrint)
            {
                mLogger.Log(noteName);
            }

            return 0;
        }

        private static int GetDatabasePath()
        {
            mConsolePrinter.Print(mTaskManager.GetDatabasePath(), "Database path");
            return 0;
        }

        private static int CreateObject(CommandLineOptions.CreateOptions options)
        {
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

            mTaskManager.CreateNote(taskId, textToWrite);
            return 0;
        }

        private static int CreateGeneralNote(string noteSubject, string textToWrite)
        {
            if (string.IsNullOrEmpty(noteSubject))
            {
                mLogger.LogError($"No task subject given to create note");
                return 1;
            }

            if (noteSubject.Length > 20)
            {
                mLogger.LogError($"Note subject {noteSubject} is too long. Must be 20 characters");
                return 1;
            }

            if (textToWrite == null)
                textToWrite = string.Empty;

            mTaskManager.CreateGeneralNote(noteSubject, textToWrite);
            return 0;
        }

        private static int RemoveObject(CommandLineOptions.RemoveOptions options)
        {
            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return RemoveTask(options.ObjectId);

                case "group":
                case "groups":
                    return RemoveTaskGroup(options.ObjectId, options.ShouldHardDelete);

                default:
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
            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return CloseTask(options.ObjectId);

                default:
                    return 1;
            }
        }

        private static int CloseTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError($"No task id given");
                return 1;
            }

            mTaskManager.CloseTask(taskId);
            return 0;
        }

        private static int OpenNote(CommandLineOptions.OpenOptions options)
        {
            if (string.IsNullOrEmpty(options.NoteName))
            {
                mLogger.LogError($"No task id given to open note");
                return 1;
            }

            mTaskManager.OpenNote(options.NoteName);
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

        private static int SetDatabasePath(ITaskManager taskManager, ConfigOptions.SetDatabasePathOptions options)
        {
            taskManager.ChangeDatabasePath(options.NewDatabasePath);
            return 0;
        }
    }
}