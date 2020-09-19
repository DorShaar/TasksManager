using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using TaskData.Notes;
using TaskData.TaskStatus;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.WorkTasks
{
    internal class WorkTask : IWorkTask
    {
        [JsonIgnore]
        private readonly ILogger<WorkTask> mLogger;

        [JsonProperty]
        private INote mNote;

        public string ID { get; }

        public string GroupName { get; set; }

        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsFinished { get => Status == Status.Closed; }

        [JsonIgnore]
        public Status Status => TaskStatusHistory.CurrentStatus;

        [JsonProperty]
        public ITaskStatusHistory TaskStatusHistory { get; }

        internal WorkTask(string id, string group, string description, ILogger<WorkTask> logger)
        {
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            ID = id ?? throw new ArgumentNullException(nameof(id));
            GroupName = group ?? throw new ArgumentNullException(nameof(group));
            Description = description ?? throw new ArgumentNullException(nameof(description));

            TaskStatusHistory = new TaskStatusHistory();
            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, "Created");

            mLogger.LogDebug($"New task id {ID} created with description: '{Description}'");
        }

        [JsonConstructor]
        internal WorkTask(
            string id, string group, string description, INote note, ITaskStatusHistory taskStatusHistory, ILogger<WorkTask> logger)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            GroupName = group ?? throw new ArgumentNullException(nameof(group));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            mNote = note ?? throw new ArgumentNullException(nameof(note));
            TaskStatusHistory = taskStatusHistory ?? throw new ArgumentNullException(nameof(taskStatusHistory));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mLogger.LogDebug($"Task id {ID} restored");
        }

        public void CloseTask(string reason)
        {
            if (Status == Status.Closed)
            {
                mLogger.LogWarning($"Task {ID}, '{Description}' is already closed");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Closed, reason);
            mLogger.LogDebug($"Task {ID}, '{Description}' closed at {TaskStatusHistory.TimeClosed}");
        }

        public void ReOpenTask(string reason)
        {
            if (Status == Status.Open)
            {
                mLogger.LogWarning($"Task {ID}, '{Description}' is already open");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, reason);
            mLogger.LogDebug($"Task {ID}, '{Description}' re-opened at {TaskStatusHistory.TimeLastOpened}");
        }

        public void MarkTaskOnWork(string reason)
        {
            if (Status == Status.OnWork)
            {
                mLogger.LogWarning($"Task {ID}, '{Description}' is already on work");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.OnWork, reason);
            mLogger.LogDebug($"Task {ID}, '{Description}' marked on work at {TaskStatusHistory.TimeLastOnWork}");
        }

        public string CreateNote(string noteDirectoryPath, string content)
        {
            string noteName = string.Empty;
            if (mNote != null)
            {
                mLogger.LogWarning($"Cannot create note since note {mNote.NotePath} is already exist");
                return noteName;
            }

            mNote = new Note(noteDirectoryPath, $"{ID}-{Description}", content);
            OpenNote();
            return mNote.NotePath;
        }

        public void OpenNote()
        {
            if (mNote == null)
            {
                mLogger.LogInformation($"Task id {ID}, '{Description}' has no note");
                return;
            }

            mNote.Open();
        }

        public string GetNote()
        {
            if (mNote == null)
            {
                mLogger.LogInformation($"Task id {ID}, '{Description}' has no note");
                return string.Empty;
            }

            return mNote.Text;
        }
    }
}