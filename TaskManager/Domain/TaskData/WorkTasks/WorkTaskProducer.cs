namespace TaskData.WorkTasks
{
    internal class WorkTaskProducer : IWorkTaskProducer
    {
        public IWorkTask ProduceTask(string id, string name, string description)
        {
            return new WorkTask(id, name, description);
        }
    }
}