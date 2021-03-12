namespace TaskData.WorkTasks.Producers
{
    internal class WorkTaskProducerFactory : IWorkTaskProducerFactory
    {
        // Here we do not use the type argument. In that solution we are producing simple tasks.
        public IWorkTaskProducer CreateProducer(string type)
        {
            return new WorkTaskProducer();
        }
    }
}