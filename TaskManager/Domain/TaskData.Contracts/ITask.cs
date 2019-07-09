namespace TaskData.Contracts
{
     public interface ITask
     {
          string ID { get; }
          string Description { get; set; }
          bool IsFinished { get; }
     }
}