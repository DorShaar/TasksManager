using Logger.Contracts;
using TaskData.Contracts;

namespace TaskData
{
    public class TaskGroupBuilder : ITaskGroupBuilder
    {
        public ITaskGroup Create(string groupName, ILogger logger)
        {
            return new TaskGroup(groupName, logger);
        }
    }
}