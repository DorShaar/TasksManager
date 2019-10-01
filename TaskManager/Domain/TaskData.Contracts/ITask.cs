namespace TaskData.Contracts
{
    public interface ITask
    {
        string ID { get; }
        string Group { get; set; }
        string Description { get; set; }
        bool IsFinished { get; }
        Status Status { get; }

        ITaskStatusHistory TaskStatusHistory { get; }

        void CloseTask(string reason);
        void ReOpenTask(string reason);
        void MarkTaskOnWork(string reason);

        // Private Notes.
        void CreateNote(string noteDirectoryPath, string content);
        void OpenNote();
        string GetNote();
    }
}