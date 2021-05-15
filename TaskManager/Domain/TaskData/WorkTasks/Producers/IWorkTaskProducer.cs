namespace TaskData.WorkTasks.Producers
{
    public interface IWorkTaskProducer
    {
        IWorkTask ProduceTask(string id, string description);
    }
}