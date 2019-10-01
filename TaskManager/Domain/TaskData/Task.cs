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

        public string Group { get; }

        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsFinished { get => Status == Status.Closed ? true : false ; }

        public Status Status { get; private set; } = Status.Open;

        public DateTime TimeCreated { get; } = DateTime.Now;
        public DateTime TimeLastOpened { get; private set; } = DateTime.Now;
        public DateTime TimeLastOnWork { get; private set; }
        public DateTime TimeClosed { get; private set; }

        public Task(string group, string description, ILogger logger)
        {
            mLogger = logger;
            ID = IDProducer.IDProducer.ProduceID();
            Group = group;
            Description = description;
            mLogger?.Log($"New task id {ID} created with description: {Description}");
        }

        [JsonConstructor]
        internal Task(ILogger logger, string id, string group, string description, Status status, INote note,
                       DateTime timeCreated, DateTime timeLastOpened, DateTime timeLastOnWork, DateTime timeClosed)
        {
            mLogger = logger;
            mNote = note;

            ID = id;
            Group = group;
            Description = description;
            Status = status;

            TimeCreated = timeCreated;
            TimeLastOpened = timeLastOpened;
            TimeLastOnWork = timeLastOnWork;
            TimeClosed = timeClosed;

            mLogger?.Log($"Task id {ID} restored");
        }

        public void CloseTask()
        {
            if (Status == Status.Closed)
            {
                mLogger?.Log($"Task {ID} is already closed");
                return;
            }

            Status = Status.Closed;
            TimeClosed = DateTime.Now;
            mLogger?.Log($"Task {ID} closed at {TimeClosed}");
        }

        public void ReOpenTask()
        {
            if (Status == Status.Open)
            {
                mLogger?.Log($"Task {ID} is already open");
                return;
            }

            Status = Status.Open;
            TimeLastOpened = DateTime.Now;
            mLogger?.Log($"Task {ID} re-opened at {TimeLastOpened}");
        }

        public void MarkTaskOnWork()
        {
            if (Status == Status.OnWork)
            {
                mLogger?.Log($"Task {ID} is already on work");
                return;
            }

            Status = Status.OnWork;
            TimeLastOnWork = DateTime.Now;
            mLogger?.Log($"Task {ID} marked on work at {TimeLastOnWork}");
        }

        public void CreateNote(string noteDirectoryPath, string content)
        {
            if (mNote != null)
            {
                mLogger.Log($"Cannot create note since note {mNote.NotePath} is already exist");
                return;
            }

            mNote = new Note(noteDirectoryPath, ID, content);
            OpenNote();
        }

        public void OpenNote()
        {
            if (mNote == null)
            {
                mLogger.LogInformation($"Task id {ID} has no note");
                return;
            }

            mNote.Open();
        }

        public string GetNote()
        {
            if (mNote == null)
            {
                mLogger.LogInformation($"Task id {ID} has no note");
                return string.Empty;
            }

            return mNote.Text;
        }
    }
}