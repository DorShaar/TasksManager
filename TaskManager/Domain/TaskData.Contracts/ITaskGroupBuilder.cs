using Logger.Contracts;

namespace TaskData.Contracts
{
     public interface ITaskGroupBuilder
     {
          ITaskGroup Create(string groupName, ILogger logger);
     }
}