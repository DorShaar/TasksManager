﻿using CommandLine;
using Composition;
using Tasker.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UI.ConsolePrinter;

namespace Tasker
{
    public class Program
    {
        private static ILogger<Program> mLogger;
        private static ConsolePrinter mConsolePrinter;
        private static TasksProvider mTasksProvider;
        private static TasksCreator mTasksCreator;
        private static TasksRemover mTasksRemover;
        private static TasksChanger mTasksChanger;
        private static NotesOpener mNotesOpener;

        public static async Task Main(string[] args)
        {
            // TODO while not exit.

            using ITaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();

            InitializeTasker(serviceProvider);

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
        }

        private static void InitializeTasker(ITaskManagerServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<Program>>();
            mConsolePrinter = serviceProvider.GetRequiredService<ConsolePrinter>();
            HttpClient httpClient = new HttpClient(); // tODO use best prtactice + dispose in the main level.

            mTasksProvider = new TasksProvider(
                httpClient, mConsolePrinter, serviceProvider.GetRequiredService<ILogger<TasksProvider>>());

            mTasksCreator = new TasksCreator(
                httpClient, serviceProvider.GetRequiredService<ILogger<TasksCreator>>());

            mTasksRemover = new TasksRemover(
                httpClient, serviceProvider.GetRequiredService<ILogger<TasksRemover>>());

            mTasksChanger = new TasksChanger(
                httpClient, serviceProvider.GetRequiredService<ILogger<TasksChanger>>());

            mNotesOpener = new NotesOpener(
                httpClient, serviceProvider.GetRequiredService<ILogger<NotesOpener>>());
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