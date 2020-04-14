using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskData.Contracts;

namespace UI.ConsolePrinter
{
    public class ConsolePrinter
    {
        private readonly ILogger mLogger;

        public ConsolePrinter(ILogger logger)
        {
            mLogger = logger;
        }

        public void PrintTasksGroup(IEnumerable<ITasksGroup> groups, bool isDetailed)
        {
            TableDataStringBuilder tableDataStringBuilder;
            if (isDetailed)
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME", "SIZE" }, mLogger);
            else
                tableDataStringBuilder = new TableDataStringBuilder(new string[] { "ID", "GROUP NAME"}, mLogger);

            foreach (ITasksGroup group in groups)
            {
                if (isDetailed)
                    tableDataStringBuilder.AppandRow(group.ID, group.Name, group.Size.ToString());
                else
                    tableDataStringBuilder.AppandRow(group.ID, group.Name);
            }

            mLogger.Log(tableDataStringBuilder.Build());
        }

        public void PrintTasks(IEnumerable<IWorkTask> tasks, bool isDetailed)
        {
            TableDataStringBuilder tableDataStringBuilder;
            if (isDetailed)
                tableDataStringBuilder = new TableDataStringBuilder(
                    new string[] 
                    {
                        "ID", "Parent", "DESCRIPTION", "STATUS", "TIME CREATED", "LAST OPENED TIME", "LAST WORK START TIME", "CLOSED TIME"
                    }, 
                    mLogger);
            else
                tableDataStringBuilder = new TableDataStringBuilder(
                    new string[]
                    {
                        "ID", "Parent", "DESCRIPTION", "STATUS"
                    },
                    mLogger);

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

            mLogger.Log(tableDataStringBuilder.Build());
        }

        public void PrintTaskInformation(IWorkTask task)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Task ID: {task.ID}");
            stringBuilder.AppendLine($"Description: {task.Description}");
            stringBuilder.AppendLine($"Status: { GetStringStatus(task.Status)}");
            stringBuilder.AppendLine($"Time created: {task.TaskStatusHistory.TimeCreated}");
            stringBuilder.AppendLine($"Last opened time: {task.TaskStatusHistory.TimeLastOpened}");
            stringBuilder.AppendLine($"Closed time: {task.TaskStatusHistory.TimeClosed}");
            stringBuilder.AppendLine($"Note: {task.GetNote()}");

            mLogger.Log(stringBuilder.ToString());
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
            mLogger.Log(header);
            mLogger.Log(data);
        }

        public void Print(IEnumerable<string> data, string header)
        {
            if (!data.Any())
                return;

            mLogger.Log(header);
            foreach (string line in data)
            {
                mLogger.Log("\t" + line);
            }
        }
    }
}