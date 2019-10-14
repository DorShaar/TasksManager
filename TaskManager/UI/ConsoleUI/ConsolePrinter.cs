using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskData.Contracts;

namespace ConsoleUI
{
    internal class ConsolePrinter
    {
        public void PrintTasksGroup(IEnumerable<ITaskGroup> groups, bool isDetailed)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(GetTaskGroupHeader(isDetailed));
            foreach (ITaskGroup group in groups)
            {
                if (isDetailed)
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

        public void PrintTasks(IEnumerable<ITask> tasks, bool isDetailed)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(GetTasksHeader(isDetailed));
            foreach (ITask task in tasks)
            {
                if (isDetailed)
                    stringBuilder.AppendLine(
                         StringFormatHelper(
                              new string[]
                              {
                                        task.ID,
                                        task.Group,
                                        task.Description,
                                        GetStringStatus(task.Status),
                                        task.TaskStatusHistory.TimeCreated.ToString(),
                                        task.TaskStatusHistory.TimeLastOpened.ToString(),
                                        task.TaskStatusHistory.TimeLastOnWork.ToString(),
                                        task.TaskStatusHistory.TimeClosed.ToString()
                              },
                              new int[] { -5, -20, -80, -10, -25, -25, -25, -25 }));
                else
                    stringBuilder.AppendLine(
                         StringFormatHelper(
                              new string[]
                              {
                                        task.ID,
                                        task.Group,
                                        task.Description,
                                        GetStringStatus(task.Status),
                              },
                              new int[] { -5, -20, -80, -10 }));
            }

            Console.WriteLine(stringBuilder.ToString());
        }

        private string GetTasksHeader(bool shouldPrintExtraDetails)
        {
            if (shouldPrintExtraDetails)
                return StringFormatHelper(new string[] { "ID", "Parent", "DESCRIPTION", "STATUS", "TIME CREATED", "LAST OPENED TIME", "LAST WORK START TIME", "CLOSED TIME" },
                                          new int[] { -5, -20, -80, -10, -25, -25, -25, -25 });
            else
                return StringFormatHelper(new string[] { "ID", "Parent", "DESCRIPTION", "STATUS" }, new int[] { -5, -20, -80, -10 });
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
            stringBuilder.AppendLine($"Status: { GetStringStatus(task.Status)}");
            stringBuilder.AppendLine($"Time created: {task.TaskStatusHistory.TimeCreated}");
            stringBuilder.AppendLine($"Last opened time: {task.TaskStatusHistory.TimeLastOpened}");
            stringBuilder.AppendLine($"Closed time: {task.TaskStatusHistory.TimeClosed}");
            stringBuilder.AppendLine($"Note: {task.GetNote()}");

            Console.WriteLine(stringBuilder.ToString());
        }

        private static string GetStringStatus(Status status)
        {
            string statusStr;
            if (status == Status.OnWork)
                statusStr = "On-Work";
            else
                statusStr = status.ToString();

            return statusStr;
        }

        public void Print(string data, string header)
        {
            Console.WriteLine(header);
            Console.WriteLine(data);
        }

        public void Print(IEnumerable<string> data, string header)
        {
            if (!data.Any())
                return;

            Console.WriteLine(header);
            foreach(string line in data)
            {
                Console.WriteLine("\t" + line);
            }
        }
    }
}