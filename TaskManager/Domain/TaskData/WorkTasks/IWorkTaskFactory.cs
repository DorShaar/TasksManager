namespace TaskData.WorkTasks
{
    public interface IWorkTaskFactory
    {
        IWorkTask Create(string groupName, string description);
    }
}