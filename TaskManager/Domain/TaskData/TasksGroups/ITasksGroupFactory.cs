using TaskData.OperationResults;
using TaskData.WorkTasks;

namespace TaskData.TasksGroups
{
     public interface ITasksGroupFactory
     {
        OperationResult<ITasksGroup> CreateGroup(string groupName);
        OperationResult<IWorkTask> CreateTask(ITasksGroup tasksGroup, string description, IWorkTaskProducer workTaskProducer);
     }
}