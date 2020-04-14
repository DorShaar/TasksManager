using Logger.Contracts;

namespace TaskData.Contracts
{
     public interface ITasksGroupBuilder
     {
          ITasksGroup Create(string groupName, ILogger logger);
     }
}