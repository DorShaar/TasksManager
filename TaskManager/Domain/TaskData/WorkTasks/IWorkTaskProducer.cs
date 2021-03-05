namespace TaskData.WorkTasks
{
    public interface IWorkTaskProducer
    {
        IWorkTask ProduceTask(string id, string name, string description);
    }
}