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

               int exitCode;

               exitCode = parser
                         .ParseArguments<
                              TaskOptions.CreateNewTaskGroupOptions,
                              TaskOptions.CreateNewTaskOptions,
                              TaskOptions.GatAllTaskGroupOptions,
                              TaskOptions.GetAllTasksOptions,
                              TaskOptions.RemoveTaskGroupOptions,
                              TaskOptions.RemoveTaskOptions,
                              TaskOptions.MoveTaskOptions,

                              ConfigOptions.SetDatabasePathOptions>(args)
                         .MapResult(
                         (TaskOptions.CreateNewTaskGroupOptions options) => CreateNewTaskGroup(taskManager, options),
                         (TaskOptions.CreateNewTaskOptions options) => CreateNewTask(taskManager, options),
                         (TaskOptions.GatAllTaskGroupOptions options) => GatAllTaskGroup(taskManager, options),
                         (TaskOptions.GetAllTasksOptions options) => GetAllTasks(taskManager, options),
                         (TaskOptions.RemoveTaskGroupOptions options) => RemoveTaskGroup(taskManager, options),
                         (TaskOptions.RemoveTaskOptions options) => RemoveTaskOptions(taskManager, options),
                         (TaskOptions.MoveTaskOptions options) => MoveTask(taskManager, options),

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
               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    taskManager.CreateNewTaskByGroupId(options.TaskGroupId, options.Description);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    taskManager.CreateNewTaskByGroupName(options.TaskGroupName, options.Description);
               else
                    taskManager.CreateNewTask(options.Description);

               return 0;
          }

          private static int GatAllTaskGroup(ITaskManager taskManager, TaskOptions.GatAllTaskGroupOptions options)
          {
               mConsolePrinter.PrintTasksGroup(taskManager.GetAllTasksGroups(), options);
               return 0;
          }

          private static int GetAllTasks(ITaskManager taskManager, TaskOptions.GetAllTasksOptions options)
          {
               IEnumerable<ITask> tasks;

               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    tasks = taskManager.GetAllTasksByGroupId(options.TaskGroupId);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    tasks = taskManager.GetAllTasksByGroupName(options.TaskGroupName);
               else
                    tasks = taskManager.GetAllTasks();

               mConsolePrinter.PrintTasks(tasks, options);
               return 0;
          }

          private static int RemoveTaskGroup(ITaskManager taskManager, TaskOptions.RemoveTaskGroupOptions options)
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

               if (!string.IsNullOrEmpty(options.TaskGroupId))
                    taskManager.MoveTaskToGroupId(options.TaskId, options.TaskGroupId);
               else if (!string.IsNullOrEmpty(options.TaskGroupName))
                    taskManager.MoveTaskToGroupName(options.TaskId, options.TaskGroupName);
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