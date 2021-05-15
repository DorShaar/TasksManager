using TaskData.TasksGroups;

namespace TaskData.WorkTasks.Producers
{
    public interface ITasksGroupProducer
    {
        ITasksGroup CreateGroup(string id, string groupName);
    }
}