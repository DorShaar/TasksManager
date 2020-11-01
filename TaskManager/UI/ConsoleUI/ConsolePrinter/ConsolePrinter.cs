using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskData.TasksGroups;
using TaskData.TaskStatus;
using TaskData.WorkTasks;

namespace UI.ConsolePrinter
{
    public class ConsolePrinter
    {
        public void PrintTasksGroup(IEnumerable<ITasksGroup> groups, bool isDetailed)
        {
            TableDataStringBuilder tableDataStringBuilder;
            if (isDetailed)
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME", "SIZE" });
            else
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME"});

            foreach (ITasksGroup group in groups)
            {
                if (isDetailed)
                    tableDataStringBuilder.AppandRow(group.ID, group.Name, group.Size.ToString());
                else
                    tableDataStringBuilder.AppandRow(group.ID, group.Name);
            }

            Console.WriteLine(tableDataStringBuilder.Build());
        }

        public void PrintTasks(IEnumerable<IWorkTask> tasks, bool isDetailed)
        {
            TableDataStringBuilder tableDataStringBuilder;
            if (isDetailed)
            {
                tableDataStringBuilder = new TableDataStringBuilder(
                   new string[]
                   {
                        "ID", "Parent", "DESCRIPTION", "STATUS", "TIME CREATED", "LAST OPENED TIME", "LAST WORK START TIME", "CLOSED TIME"
                   });
            }
            else
            {
                tableDataStringBuilder = new TableDataStringBuilder(
                   new string[]
                   {
                        "ID", "Parent", "DESCRIPTION", "STATUS"
                   });
            }

            foreach (IWorkTask task in tasks)
            {
                if (isDetailed)
                {
                    tableDataStringBuilder.AppandRow(
                                        task.ID,
                                        task.GroupName,
                                        task.Description,
                                        GetStringStatus(task.Status),
                                        task.TaskStatusHistory.TimeCreated.ToString(),
                                        task.TaskStatusHistory.TimeLastOpened.ToString(),
                                        task.TaskStatusHistory.TimeLastOnWork.ToString(),
                                        task.TaskStatusHistory.TimeClosed.ToString());
                }
                else
                {
                    tableDataStringBuilder.AppandRow(
                                        task.ID,
                                        task.GroupName,
                                        task.Description,
                                        GetStringStatus(task.Status));
                }
            }

            Console.WriteLine(tableDataStringBuilder.Build());
        }

        public void PrintTaskInformation(IWorkTask task)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Task ID: ").AppendLine(task.ID)
                .Append("Description: ").AppendLine(task.Description)
                .Append("Status: ").AppendLine(GetStringStatus(task.Status))
                .Append("Time created: ").Append(task.TaskStatusHistory.TimeCreated).AppendLine()
                .Append("Last opened time: ").Append(task.TaskStatusHistory.TimeLastOpened).AppendLine()
                .Append("Closed time: ").Append(task.TaskStatusHistory.TimeClosed).AppendLine()
                .Append("Note: ").AppendLine(task.GetNote().Value);

            Console.WriteLine(stringBuilder.ToString());
        }

        private static string GetStringStatus(Status status)
        {
            return status == Status.OnWork ? "On-Work" : status.ToString();
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
            foreach (string line in data)
            {
                Console.WriteLine("\t" + line);
            }
        }
    }
}