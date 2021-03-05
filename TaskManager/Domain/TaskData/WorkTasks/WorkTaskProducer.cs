namespace TaskData.WorkTasks
{
    internal class WorkTaskProducer : IWorkTaskProducer
    {
        public IWorkTask ProduceTask(string id, string description)
        {
            return new WorkTask(id, description);
        }
    }
}