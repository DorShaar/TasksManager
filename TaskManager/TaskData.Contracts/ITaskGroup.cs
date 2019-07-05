namespace TaskData.Contracts
{
     public interface ITaskGroup
     {
          string GroupName { get; set; }
          ITask GetTask(string id);
          void AddTask(ITask task);
          void RemoveTask(string id);
          void RemoveTask(params string[] ids);
          void UpdateTask(ITask task);

          int Size { get; }
     }
}