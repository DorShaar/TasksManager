namespace TaskData.Interfaces
{
     public interface ITaskGroup
     {
          string Name { get; set; }
          ITask GetTask(string id);
          void AddTask(ITask task);
          void RemoveTask(string id);
          void UpdateTask(ITask task);
     }
}