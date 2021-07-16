using TaskData.TasksGroups;

namespace TaskData.TasksGroups.Producers
{
    public class TasksGroupProducer : ITasksGroupProducer
    {
        public ITasksGroup CreateGroup(string id, string groupName)
        {
            return new TasksGroup(id, groupName);
        }
    }
}