using System;
using System.Collections.Generic;
using System.Linq;
using Tasker.Resources;

namespace UI.ConsolePrinter
{
    public class ConsolePrinter
    {
        public void PrintTasksGroup(IEnumerable<TasksGroupResource> groups, bool isDetailed)
        {
            TableDataStringBuilder tableDataStringBuilder;
            if (isDetailed)
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME", "SIZE" });
            else
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME" });

            foreach (TasksGroupResource group in groups)
            {
                if (isDetailed)
                    tableDataStringBuilder.AppandRow(group.GroupId, group.GroupName, group.Size.ToString());
                else
                    tableDataStringBuilder.AppandRow(group.GroupId, group.GroupName);
            }

            Console.WriteLine(tableDataStringBuilder.Build());
        }

        public void PrintTasks(IEnumerable<WorkTaskResource> tasks, bool isDetailed)
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
                        "ID", "Parent", "DESCRIPTION", "STATUS", string.Empty, string.Empty, string.Empty, string.Empty
                   });
            }

            foreach (WorkTaskResource task in tasks)
            {
                tableDataStringBuilder.AppandRow(
                                    task.TaskId,
                                    task.GroupName,
                                    task.Description,
                                    task.Status,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty);
            }

            Console.WriteLine(tableDataStringBuilder.Build());
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