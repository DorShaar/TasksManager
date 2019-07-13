using System;
using System.Collections.Generic;
using System.Text;
using TaskData.Contracts;

namespace ConsoleUI
{
     internal class ConsolePrinter
     {
          public void PrintTasksGroup(IEnumerable<ITaskGroup> groups, TaskOptions.GatAllTaskGroupOptions options)
          {
               StringBuilder stringBuilder = new StringBuilder();

               stringBuilder.AppendLine(GetTaskGroupHeader(options.IsDetailed));
               foreach (ITaskGroup group in groups)
               {
                    if (options.IsDetailed)
                         stringBuilder.AppendLine(
                              StringFormatHelper(new string[] { group.ID, group.GroupName, group.Size.ToString() }, new int[] { -5, -25, -10 }));
                    else
                         stringBuilder.AppendLine(
                              StringFormatHelper(new string[] { group.ID, group.GroupName, }, new int[] { -5, -25 }));
               }

               Console.WriteLine(stringBuilder.ToString());
          }

          private string GetTaskGroupHeader(bool shouldPrintExtraDetails)
          {
               if (shouldPrintExtraDetails)
                    return StringFormatHelper(new string[] { "ID", "GROUP NAME", "SIZE" }, new int[] { -5, -25, -10 });
               else
                    return StringFormatHelper(new string[] { "ID", "GROUP NAME" }, new int[] { -5, -25 });
          }

          public void PrintTasks(IEnumerable<ITask> tasks, TaskOptions.GetAllTasksOptions options)
          {
               StringBuilder stringBuilder = new StringBuilder();

               stringBuilder.AppendLine(GetTasksHeader(options.IsDetailed));
               foreach (ITask task in tasks)
               {
                    if (options.IsDetailed)
                         stringBuilder.AppendLine(
                              StringFormatHelper(
                                   new string[]
                                   {
                                        task.ID,
                                        task.Description,
                                        GetStringStatus(task.IsFinished),
                                        task.TimeCreated.ToString(),
                                        task.TimeLastOpened.ToString(),
                                        task.TimeClosed.ToString()
                                   },
                                   new int[] { -5, -80, -10, -25, -25, -25 }));
                    else
                         stringBuilder.AppendLine(
                              StringFormatHelper(
                                   new string[]
                                   {
                                        task.ID,
                                        task.Description,
                                        GetStringStatus(task.IsFinished),
                                   },
                                   new int[] { -5, -80, -10 }));
               }

               Console.WriteLine(stringBuilder.ToString());
          }

          private string GetTasksHeader(bool shouldPrintExtraDetails)
          {
               if (shouldPrintExtraDetails)
                    return StringFormatHelper(new string[] { "ID", "DESCRIPTION", "STATUS", "TIME CREATED", "LAST OPENED TIME ", "CLOSED TIME" }, 
                                              new int[] { -5, -80, -10, -25, -25, -25 });
               else
                    return StringFormatHelper(new string[] { "ID", "DESCRIPTION", "STATUS" }, new int[] { -5, -80, -10 });
          }

          private string StringFormatHelper(string[] args, int[] argsLength)
          {
               return string.Format(StringFormatBuilder(args, argsLength), args);
          }

          /// <summary>
          /// Builds string of few {index, length}, which to string.Format.
          /// </summary>
          /// <param name="args"></param>
          /// <param name="argsLength"></param>
          /// <returns></returns>
          private string StringFormatBuilder(string[] args, int[] argsLength)
          {
               StringBuilder stringBuilder = new StringBuilder();
               if (args.Length != argsLength.Length)
                    throw new ArgumentException($"Number of argument is {args.Length} which is different from size of length arguments {argsLength.Length}");

               for (int i = 0; i < args.Length; ++i)
               {
                    stringBuilder.Append("{");
                    stringBuilder.Append(i);
                    stringBuilder.Append($",{argsLength[i]}");
                    stringBuilder.Append("}");
                    stringBuilder.Append(" ");
               }

               return stringBuilder.ToString();
          }

          public void PrintTaskInformation(ITask task)
          {
               StringBuilder stringBuilder = new StringBuilder();

               stringBuilder.AppendLine($"Task ID: {task.ID}");
               stringBuilder.AppendLine($"Description: {task.Description}");
               stringBuilder.AppendLine($"Status: {GetStringStatus(task.IsFinished)}");
               stringBuilder.AppendLine($"Time created: {task.TimeCreated}");
               stringBuilder.AppendLine($"Last opened time: {task.TimeLastOpened}");
               stringBuilder.AppendLine($"Closed time: {task.TimeClosed}");
               stringBuilder.AppendLine($"Note: {task.GetNote()}");

               Console.WriteLine(stringBuilder.ToString());
          }

          private static string GetStringStatus(bool isFinished)
          {
               return isFinished ? "Done" : "Open";
          }
     }
}