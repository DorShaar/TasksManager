using Logger.Contracts;
using TaskData.Contracts;

namespace TaskData
{
    public class TaskGroupBuilder : ITasksGroupBuilder
    {
        public ITasksGroup Create(string groupName, ILogger logger)
        {
            return new TaskGroup(groupName, logger);
        }
    }
}