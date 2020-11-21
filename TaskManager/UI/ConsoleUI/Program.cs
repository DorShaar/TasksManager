using CommandLine;
using Composition;
using Tasker.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tasker.UserInputs;
using Tasker.Extensions;
using Tasker.TaskerWorkers;

namespace Tasker
{
    public class Program
    {
        private static ILogger<Program> mLogger;
        private static TasksProvider mTasksProvider;
        private static TasksCreator mTasksCreator;
        private static TasksRemover mTasksRemover;
        private static TasksChanger mTasksChanger;
        private static NotesOpener mNotesOpener;

        public static async Task Main(string[] args)
        {
            using ITaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();

            InitializeTasker(serviceProvider);

            await RunTasker(args).ConfigureAwait(false);
        }

        private static void InitializeTasker(ITaskManagerServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<Program>>();
            mTasksProvider = serviceProvider.GetRequiredService<TasksProvider>();
            mTasksCreator = serviceProvider.GetRequiredService<TasksCreator>();
            mTasksRemover = serviceProvider.GetRequiredService<TasksRemover>();
            mTasksChanger = serviceProvider.GetRequiredService<TasksChanger>();
            mNotesOpener = serviceProvider.GetRequiredService<NotesOpener>();
        }

        private static async Task RunTasker(string[] args)
        {
            UserInput userInput = new UserInput();
            using Parser parser = new Parser(config => config.HelpWriter = Console.Out);

            while (!ShouldEndApplication(args))
            {
                int exitCode = await ParseArgument(parser, args).ConfigureAwait(false);

                if (exitCode != 0)
                    mLogger.LogInformation($"Problem accured, exit code: {exitCode}");

                args = GetUserInputArguments(userInput);
            }
        }

        private static string[] GetUserInputArguments(UserInput userInput)
        {
            userInput.GetUserInput();
            string[] args = userInput.GetArguments();

            if (args[0].Equals("tasker", StringComparison.InvariantCultureIgnoreCase))
                return args.Slice(1, args.Length);

            return args;
        }

        private static bool ShouldEndApplication(string[] userInputArguemnts)
        {
            if (userInputArguemnts.Length == 0)
                return false;

            string userInput = userInputArguemnts[0];

            if (string.IsNullOrWhiteSpace(userInput))
                return false;

            bool shouldEndApplication =
                userInput.Contains("x", StringComparison.InvariantCultureIgnoreCase) ||
                userInput.Contains("q", StringComparison.InvariantCultureIgnoreCase) ||
                userInput.Contains("exit", StringComparison.InvariantCultureIgnoreCase) ||
                userInput.Contains("stop", StringComparison.InvariantCultureIgnoreCase) ||
                userInput.Contains("bye", StringComparison.InvariantCultureIgnoreCase);

            if (shouldEndApplication)
                mLogger.LogInformation("Tasker application was terminated by the user");

            return shouldEndApplication;
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
                CommandLineOptions.OpenNoteOptions>(args).MapResult<
                    CommandLineOptions.GetOptions,
                    CommandLineOptions.CreateOptions,
                    CommandLineOptions.RemoveOptions,
                    CommandLineOptions.CloseOptions,
                    CommandLineOptions.MoveTaskOptions,
                    CommandLineOptions.ReOpenTaskOptions,
                    CommandLineOptions.OnWorkTaskOptions,
                    CommandLineOptions.OpenNoteOptions,
                    Task<int>>(
                    mTasksProvider.GetObject,
                    mTasksCreator.CreateObject,
                    mTasksRemover.RemoveObject,
                    mTasksChanger.CloseTask,
                    mTasksChanger.MoveTask,
                    mTasksChanger.ReOpenTask,
                    mTasksChanger.MarkTaskAsOnWork,
                    mNotesOpener.OpenNote,
                    (parseError) => Task.FromResult(1));
        }
    }
}