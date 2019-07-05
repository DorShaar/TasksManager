namespace TaskData.Contracts
{
     public interface ITask
     {
          string ID { get; }
          ITaskGroup TaskFamily { get; set; }
          string Description { get; set; }
          bool IsFinished { get; }
     }
}