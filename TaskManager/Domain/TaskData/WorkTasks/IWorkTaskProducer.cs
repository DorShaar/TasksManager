namespace TaskData.WorkTasks
{
    public interface IWorkTaskProducer
    {
        IWorkTask ProduceTask(string id, string description);
    }
}