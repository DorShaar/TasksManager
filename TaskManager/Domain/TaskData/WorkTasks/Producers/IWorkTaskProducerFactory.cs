namespace TaskData.WorkTasks.Producers
{
    public interface IWorkTaskProducerFactory
    {
        IWorkTaskProducer CreateProducer(string type);
    }
}