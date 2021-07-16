using TaskData.TasksGroups;

namespace TaskData.TasksGroups.Producers
{
    public interface ITasksGroupProducer
    {
        ITasksGroup CreateGroup(string id, string groupName);
    }
}