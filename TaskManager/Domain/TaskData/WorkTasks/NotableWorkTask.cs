using Newtonsoft.Json;
using TaskData.Notes;
using TaskData.OperationResults;
using TaskData.TaskStatus;
using Triangle;

namespace TaskData.WorkTasks
{
    internal class NotableWorkTask : WorkTask, INotable
    {
        [JsonProperty]
        private INote mNote;

        internal NotableWorkTask(string id, string description) : base(id, description)
        {
        }

        [JsonConstructor]
        internal NotableWorkTask(string id,
            string groupName,
            string description,
            INote note,
            ITaskStatusHistory taskStatusHistory,
            TaskTriangle taskTriangle) : base (id, groupName, description, taskStatusHistory, taskTriangle)
        {
            mNote = note;
        }

        public OperationResult CreateNote(string noteDirectoryPath, string content)
        {
            if (mNote != null)
            {
                return new OperationResult(false, $"Cannot create note since note {mNote.NotePath} is already exist");
            }

            mNote = new Note(noteDirectoryPath, $"{ID}-{Description}", content);
            return new OperationResult(true);
        }

        public OperationResult<string> GetNote()
        {
            if (mNote == null)
                return new OperationResult<string>(false, $"Task id {ID}, '{Description}' has no note");

            return new OperationResult<string>(true, mNote.Text);
        }
    }
}