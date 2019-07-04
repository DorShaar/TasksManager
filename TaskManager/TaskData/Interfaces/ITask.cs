namespace TaskData.Interfaces
{
     public interface ITask
     {
          string ID { get; }
          ITaskGroup TaskFamily { get; set; }
          string Description { get; set; }
          bool IsFinished { get; }
     }
}