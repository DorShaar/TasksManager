using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using TaskData.OperationResults;
using TaskData.TaskStatus;
using Triangle;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.WorkTasks
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WorkTask : IWorkTask
    {
        [JsonProperty]
        public string ID { get; }

        [JsonProperty]
        public string GroupName { get; set; }

        [JsonProperty]
        public string Description { get; set; } = string.Empty;

        public bool IsFinished { get => Status == Status.Closed; }

        public Status Status => TaskStatusHistory.CurrentStatus;

        [JsonProperty]
        public ITaskStatusHistory TaskStatusHistory { get; }

        [JsonProperty]
        public TaskTriangle TaskMeasurement { get; private set; }

        internal WorkTask(string id, string groupName, string description)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
            Description = description ?? throw new ArgumentNullException(nameof(description));

            TaskStatusHistory = new TaskStatusHistory();
            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, "Created");
        }

        [JsonConstructor]
        internal WorkTask(string id,
            string groupName,
            string description,
            ITaskStatusHistory taskStatusHistory,
            TaskTriangle taskTriangle)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            TaskStatusHistory = taskStatusHistory ?? throw new ArgumentNullException(nameof(taskStatusHistory));
            TaskMeasurement = taskTriangle;
        }

        public OperationResult CloseTask(string reason)
        {
            if (Status == Status.Closed)
                return new OperationResult(false, $"Task {ID}, '{Description}' is already closed");

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Closed, reason);
            return new OperationResult(true, $"Task {ID}, '{Description}' closed at {TaskStatusHistory.TimeClosed}");
        }

        public OperationResult ReOpenTask(string reason)
        {
            if (Status == Status.Open)
                return new OperationResult(false, $"Task {ID}, '{Description}' is already open");

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, reason);
            return new OperationResult(true, $"Task {ID}, '{Description}' re-opened at {TaskStatusHistory.TimeLastOpened}");
        }

        public OperationResult MarkTaskOnWork(string reason)
        {
            if (Status == Status.OnWork)
                return new OperationResult(false, $"Task {ID}, '{Description}' is already on work");

            TaskStatusHistory.AddHistory(DateTime.Now, Status.OnWork, reason);
            return new OperationResult(true, $"Task {ID}, '{Description}' marked on work at {TaskStatusHistory.TimeLastOnWork}");
        }

        public void SetMeasurement(TaskTriangle measurement)
        {
            TaskMeasurement = measurement;
        }
    }
}