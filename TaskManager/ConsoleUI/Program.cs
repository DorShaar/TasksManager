using CommandLine;
using Composition;
using Logger.Contracts;
using System;
using System.Collections.Generic;
using TaskData.Contracts;
using TaskManager.Contracts;

namespace ConsoleUI
{
     public class Program
     {
          private static ILogger mLogger;

          public static void Main(string[] args)
          {
               TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
               mLogger = serviceProvider.GetLoggerService();
               ITaskManager taskManager = serviceProvider.GetTaskManagerService();

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
                         (Options.CreateNewTaskGroupOptions options) => CreateNewTaskGroup(taskManager, options),
                         (Options.CreateNewTaskOptions options) => CreateNewTask(taskManager, options),
                         (Options.GatAllTaskGroupOptions options) => GatAllTaskGroup(taskManager, options),
                         (Options.GetAllTasksOptions options) => GetAllTasks(taskManager, options),
                         (Options.RemoveTaskGroupOptions options) => RemoveTaskGroup(taskManager, options),
                         (parserErrors) => 1
               );

               if (exitCode != 0)
                    Console.WriteLine($"Finished executing with exit code: {exitCode}");
          }

          private static int CreateNewTaskGroup(ITaskManager taskManager, Options.CreateNewTaskGroupOptions options)
          {
               if (string.IsNullOrEmpty(options.TaskGroupName))
               {
                    mLogger.LogError($"{nameof(options.TaskGroupName)} is null or empty");
                    return 1;
               }
               else
               {
                    taskManager.CreateNewTaskGroup(options.TaskGroupName);
               }

               return 0;
          }

          private static int CreateNewTask(ITaskManager taskManager, Options.CreateNewTaskOptions options)
          {
               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    taskManager.CreateNewTaskByGroupId(options.TaskGroupId, options.Description);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    taskManager.CreateNewTaskByGroupName(options.TaskGroupName, options.Description);
               else
                    taskManager.CreateNewTask(options.Description);

               return 0;
          }

          private static int GatAllTaskGroup(ITaskManager taskManager, Options.GatAllTaskGroupOptions options)
          {
               IEnumerable<ITaskGroup> groups = taskManager.GetAllTasksGroups();
               foreach(ITaskGroup group in groups)
               {
                    string info = $"ID: {group.ID}          Name: {group.GroupName}";
                    if (options.IsDetailed)
                         info += $"       Size: {group.Size}";

                    mLogger.Log(info);
               }

               return 0;
          }

          private static int GetAllTasks(ITaskManager taskManager, Options.GetAllTasksOptions options)
          {
               IEnumerable<ITask> tasks;

               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    tasks = taskManager.GetAllTasksByGroupId(options.TaskGroupId);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    tasks = taskManager.GetAllTasksByGroupName(options.TaskGroupName);
               else
                    tasks = taskManager.GetAllTasks();

               foreach (ITask task in tasks)
               {
                    string info = $"ID: {task.ID}          Description: {task.Description}";
                    if (options.IsDetailed)
                         info += $"       Status: {GetStringStatus(task.IsFinished)}";

                    mLogger.Log(info);
               }

               return 0;
          }

          private static string GetStringStatus(bool isFinished)
          {
               return isFinished ? "Done" : "Open";
          }

          private static int RemoveTaskGroup(ITaskManager taskManager, Options.RemoveTaskGroupOptions options)
          {
               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    taskManager.RemoveTaskGroupById(options.TaskGroupId);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    taskManager.RemoveTaskGroupByName(options.TaskGroupName);
               else
               {
                    mLogger.LogError($"No group name or group id given");
                    return 1;
               }

               return 0;
          }
     }
}
