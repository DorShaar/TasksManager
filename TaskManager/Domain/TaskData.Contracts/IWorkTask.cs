namespace TaskData.Contracts
{
    public interface IWorkTask
    {
        string ID { get; }
        string GroupName { get; set; }
        string Description { get; set; }
        bool IsFinished { get; }
        Status Status { get; }

        ITaskStatusHistory TaskStatusHistory { get; }

        void CloseTask(string reason);
        void ReOpenTask(string reason);
        void MarkTaskOnWork(string reason);

        // Private Notes.
        string CreateNote(string noteDirectoryPath, string content);
        void OpenNote();
        string GetNote();
    }
}