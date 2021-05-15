using TaskData.TasksGroups;

namespace TaskData.WorkTasks.Producers
{
    public class TasksGroupProducer : ITasksGroupProducer
    {
        public ITasksGroup CreateGroup(string id, string groupName)
        {
            return new TasksGroup(id, groupName);
        }
    }
}