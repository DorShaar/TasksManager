using CommandLine;
using Composition;
using ConsoleUI.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TaskData.Notes;
using TaskData.WorkTasks;
using UI.ConsolePrinter;

namespace ConsoleUI
{
    public class Program
    {
        private static ILogger<Program> mLogger;
        private static ConsolePrinter mConsolePrinter;
        private static TasksProvider mTasksProvider;
        private static TasksCreator mTasksCreator;

        public static async Task Main(string[] args)
        {
            // TODO while not exit.

            using ITaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
            mLogger = serviceProvider.GetRequiredService<ILogger<Program>>();
            mConsolePrinter = serviceProvider.GetRequiredService<ConsolePrinter>();
            HttpClient httpClient = new HttpClient();

            mTasksProvider = new TasksProvider(
                httpClient, mConsolePrinter, serviceProvider.GetRequiredService<ILogger<TasksProvider>>());

            mTasksCreator = new TasksCreator(
                httpClient, mConsolePrinter, serviceProvider.GetRequiredService<ILogger<TasksCreator>>());

            int exitCode = 1;
            using (Parser parser = new Parser(config => config.HelpWriter = Console.Out))
            {
                if (args.Length == 0)
                {
                    parser.ParseArguments<CommandLineOptions>(new[] { "--help" });
                    return;
                }

                exitCode = await ParseArgument(parser, args).ConfigureAwait(false);
            }

            if (exitCode != 0)
                mLogger.LogInformation($"Finished executing with exit code: {exitCode}");

            mTasksProvider.Dispose();
        }

        private static Task<int> ParseArgument(Parser parser, string[] args)
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
                CommandLineOptions.OpenNoteOptions>(args).MapResult<
                    CommandLineOptions.GetOptions,
                    CommandLineOptions.CreateOptions,
                    CommandLineOptions.RemoveOptions,
                    CommandLineOptions.CloseOptions,
                    CommandLineOptions.MoveTaskOptions,
                    CommandLineOptions.ReOpenTaskOptions,
                    CommandLineOptions.OnWorkTaskOptions,
                    CommandLineOptions.GetInformationTaskOptions,
                    CommandLineOptions.OpenNoteOptions,
                    Task<int>>(
                    mTasksProvider.GetObject,
                    mTasksCreator.CreateObject,
                    RemoveObject,
                    CloseTask,
                    MoveTask,
                    ReOpenTask,
                    MarkTaskAsOnWork,
                    GetTaskInformation,
                    OpenNote,
                    (parseError) => Task.FromResult(1));
        }

        private static async Task<int> RemoveObject(CommandLineOptions.RemoveOptions options)
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

        private static async Task<int> RemoveTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given to remove");
                return 1;
            }

            mTaskManager.RemoveTask(taskId);
            return 0;
        }

        private static async Task<int> RemoveTaskGroup(string taskGroup, bool shouldHardDelete)
        {
            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError("No group name or group id given");
                return 1;
            }

            mLogger.LogDebug("Are you sure you want to delete that group with all of its inner tasks? If so, press y");
            string userInput = Console.ReadLine();
            if (!string.Equals(userInput, "y", StringComparison.OrdinalIgnoreCase))
            {
                mLogger.LogDebug($"Group {taskGroup} was not deleted");
                return 0;
            }

            mTaskManager.RemoveTaskGroup(taskGroup, !shouldHardDelete);
            return 0;
        }

        private static async Task<int> CloseTask(CommandLineOptions.CloseOptions options)
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

        private static async Task<int> CloseTask(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            mTaskManager.CloseTask(taskId, reason ?? string.Empty);
            return 0;
        }

        private static async Task<int> OpenNote(CommandLineOptions.OpenNoteOptions options)
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

        private static async Task<int> OpenNote(string noteName)
        {
            INote note = GetNote(mTaskManager.NotesTasksDatabase, noteName);
            note?.Open();

            return 0;
        }

        private static async Task<int> OpenGeneralNote(string noteName)
        {
            INote note = GetNote(mTaskManager.NotesRootDatabase, noteName);
            note?.Open();

            return 0;
        }

        private static async Task<int> MoveTask(CommandLineOptions.MoveTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return MoveTask(options.TaskId, options.TaskGroup);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private static async Task<int> MoveTask(string taskId, string taskGroup)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given to move");
                return 1;
            }

            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError("No group name or group id given");
                return 1;
            }

            mTaskManager.MoveTaskToGroup(taskId, taskGroup);
            return 0;
        }

        private static async Task<int> ReOpenTask(CommandLineOptions.ReOpenTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return ReOpenTask(options.TaskId, options.Reason);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private static async Task<int> ReOpenTask(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            mTaskManager.ReOpenTask(taskId, reason ?? string.Empty);
            return 0;
        }

        private static async Task<int> MarkTaskAsOnWork(CommandLineOptions.OnWorkTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return MarkTaskAsOnWork(options.TaskId, options.Reason);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private static async Task<int> MarkTaskAsOnWork(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            mTaskManager.MarkTaskOnWork(taskId, reason ?? string.Empty);
            return 0;
        }

        private static async Task<int> GetTaskInformation(CommandLineOptions.GetInformationTaskOptions options)
        {
            if (string.IsNullOrEmpty(options.TaskId))
            {
                mLogger.LogError("No task id given to create note");
                return 1;
            }

            mConsolePrinter.PrintTaskInformation(
                 mTaskManager.GetAllTasks((IWorkTask task) => task.ID == options.TaskId).First());
            return 0;
        }
    }
}