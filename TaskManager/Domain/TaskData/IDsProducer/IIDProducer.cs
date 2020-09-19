namespace TaskData.IDsProducer
{
    public interface IIDProducer
    {
        void SetNextID(int lastID);
        string ProduceID();
        string PeekForNextId();
    }
}