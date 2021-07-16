namespace TaskData.WorkTasks.Producers
{
    public class WorkTaskProducer : IWorkTaskProducer
    {
        public IWorkTask ProduceTask(string id, string description)
        {
            return new WorkTask(id, description);
        }
    }
}