using CommandLine;
using Logger;
using System;
using TaskManager.Contracts;

namespace ConsoleUI
{
     public class Program
     {
          public static void Main(string[] args)
          {
               ITaskManager taskManager = new TaskManager.TaskManager("NotedNTasks.db", new ConsoleLogger());

               var parser = new Parser(config => config.HelpWriter = Console.Out);
               if (args.Length == 0)
               {
                    parser.ParseArguments<Options>(new[] { "--help" });
                    return;
               }

               int exitCode;

               exitCode = parser
                         .ParseArguments<
                              Options.CreateNewTaskGroupOptions,
                              Options.CreateNewTaskOptions,
                              Options.GatAllTaskGroupOptions,
                              Options.GetAllTasksOptions,
                              Options.RemoveTaskGroupOptions>(args)
                         .MapResult(
                         (Options.CreateNewTaskGroupOptions opts) => RunCommitCommand(opts),
                         (CloneOptions opts) => RunCloneCommand(opts),
                         //(AddOptions opts) => RunAddCommand(opts), 
                         (parserErrors) => 1
               );

               if (exitCode != 0)
                    Console.WriteLine($"Finished executing with exit code: {exitCode}");
          }
     }
}
