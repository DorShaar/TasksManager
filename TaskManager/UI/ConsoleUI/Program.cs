using CommandLine;
using Composition;
using Logger.Contracts;
using System;
using System.Collections.Generic;
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
            TaskOptions.CreateNewTaskGroupOptions,
            TaskOptions.GatAllTaskGroupOptions,
            TaskOptions.RemoveTaskGroupOptions,

            TaskOptions.CreateNewTaskOptions,
            TaskOptions.GetAllTasksOptions,
            TaskOptions.CloseTasksOptions,
            TaskOptions.RemoveTaskOptions,
            TaskOptions.MoveTaskOptions,
            TaskOptions.ReOpenTaskOptions,
            TaskOptions.GetInformationTaskOptions,

            TaskOptions.CreateNoteOptions,
            TaskOptions.OpenNoteOptions,
            TaskOptions.GetNoteOptions,

            ConfigOptions.SetDatabasePathOptions>(args).MapResult(
                 (TaskOptions.CreateNewTaskGroupOptions options) => CreateNewTaskGroup(taskManager, options),
                 (TaskOptions.GatAllTaskGroupOptions options) => GatAllTaskGroup(taskManager, options),
                 (TaskOptions.RemoveTaskGroupOptions options) => RemoveTaskGroup(taskManager, options),

                 (TaskOptions.CreateNewTaskOptions options) => CreateNewTask(taskManager, options),
                 (TaskOptions.GetAllTasksOptions options) => GetAllTasks(taskManager, options),
                 (TaskOptions.CloseTasksOptions options) => CloseTask(taskManager, options),
                 (TaskOptions.RemoveTaskOptions options) => RemoveTaskOptions(taskManager, options),
                 (TaskOptions.MoveTaskOptions options) => MoveTask(taskManager, options),
                 (TaskOptions.ReOpenTaskOptions options) => ReOpenTask(taskManager, options),
                 (TaskOptions.GetInformationTaskOptions options) => GetTaskInformation(taskManager, options),

                 (TaskOptions.CreateNoteOptions options) => CreateNote(taskManager, options),
                 (TaskOptions.OpenNoteOptions options) => OpenNote(taskManager, options),
                 (TaskOptions.GetNoteOptions options) => GetNote(taskManager, options),

                 (ConfigOptions.SetDatabasePathOptions options) => SetDatabasePath(taskManager, options),
                 (parserErrors) => 1
            );

            if (exitCode != 0)
                Console.WriteLine($"Finished executing with exit code: {exitCode}");
        }

        private static int CreateNewTaskGroup(ITaskManager taskManager, TaskOptions.CreateNewTaskGroupOptions options)
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
                taskManager.CreateNewTask(options.TaskGroup, options.Description);
            else
                taskManager.CreateNewTask(options.Description);

            return 0;
        }

        private static int GatAllTaskGroup(ITaskManager taskManager, TaskOptions.GatAllTaskGroupOptions options)
        {
            mConsolePrinter.PrintTasksGroup(taskManager.GetAllTasksGroups(), options);
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
            }
            else
                allTasks = taskManager.GetAllTasks();

            IEnumerable<ITask> tasksToPrint = allTasks;
            if (!options.ShouldPrintAll)
                tasksToPrint = allTasks.Where(task => task.IsFinished == false);

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

        private static int RemoveTaskGroup(ITaskManager taskManager, TaskOptions.RemoveTaskGroupOptions options)
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

        private static int CreateNote(ITaskManager taskManager, TaskOptions.CreateNoteOptions options)
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

        private static int OpenNote(ITaskManager taskManager, TaskOptions.OpenNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to open note");
                return 1;
            }

            taskManager.OpenNote(options.TaskId);
            return 0;
        }

        private static int GetNote(ITaskManager taskManager, TaskOptions.GetNoteOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError($"No task id given to get note");
                return 1;
            }

            mLogger.Log(taskManager.GetNote(options.TaskId));
            return 0;
        }

        private static int SetDatabasePath(ITaskManager taskManager, ConfigOptions.SetDatabasePathOptions options)
        {
            taskManager.ChangeDatabasePath(options.NewDatabasePath);
            return 0;
        }
    }
}