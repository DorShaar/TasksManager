using Logger.Contracts;
using Newtonsoft.Json;
using System;
using TaskData.Contracts;

namespace TaskData
{
    public class Task : ITask
    {
        [JsonProperty]
        private readonly ILogger mLogger;

        [JsonProperty]
        private INote mNote;

        public string ID { get; }

        public string Group { get; set; }

        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsFinished { get => Status == Status.Closed ? true : false ; }

        [JsonIgnore]
        public Status Status => TaskStatusHistory.CurrentStatus;

        [JsonProperty]
        public ITaskStatusHistory TaskStatusHistory { get; }

        public Task(string group, string description, ILogger logger)
        {
            mLogger = logger;
            ID = IDProducer.IDProducer.ProduceID();
            Group = group;
            Description = description;

            TaskStatusHistory = new TaskStatusHistory();
            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, "Created");

            mLogger?.Log($"New task id {ID} created with description: '{Description}'");
        }

        [JsonConstructor]
        internal Task(ILogger logger, string id, string group, string description, INote note, ITaskStatusHistory taskStatusHistory)
        {
            mLogger = logger;
            mNote = note;

            ID = id;
            Group = group;
            Description = description;
            TaskStatusHistory = taskStatusHistory;

            mLogger?.Log($"Task id {ID} restored");
        }

        public void CloseTask(string reason)
        {
            if (Status == Status.Closed)
            {
                mLogger?.Log($"Task {ID}, '{Description}' is already closed");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Closed, reason);
            mLogger?.Log($"Task {ID}, '{Description}' closed at {TaskStatusHistory.TimeClosed}");
        }

        public void ReOpenTask(string reason)
        {
            if (Status == Status.Open)
            {
                mLogger?.Log($"Task {ID}, '{Description}' is already open");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.Open, reason);
            mLogger?.Log($"Task {ID}, '{Description}' re-opened at {TaskStatusHistory.TimeLastOpened}");
        }

        public void MarkTaskOnWork(string reason)
        {
            if (Status == Status.OnWork)
            {
                mLogger?.Log($"Task {ID}, '{Description}' is already on work");
                return;
            }

            TaskStatusHistory.AddHistory(DateTime.Now, Status.OnWork, reason);
            mLogger?.Log($"Task {ID}, '{Description}' marked on work at {TaskStatusHistory.TimeLastOnWork}");
        }

        public string CreateNote(string noteDirectoryPath, string content)
        {
            string noteName = string.Empty;
            if (mNote != null)
            {
                mLogger.Log($"Cannot create note since note {mNote.NotePath} is already exist");
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