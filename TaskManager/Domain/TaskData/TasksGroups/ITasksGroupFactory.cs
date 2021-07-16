using TaskData.OperationResults;
using TaskData.TasksGroups.Producers;
using TaskData.WorkTasks;
using TaskData.WorkTasks.Producers;

namespace TaskData.TasksGroups
{
     public interface ITasksGroupFactory
     {
        OperationResult<ITasksGroup> CreateGroup(string groupName, ITasksGroupProducer tasksGroupProducer);
        OperationResult<IWorkTask> CreateTask(ITasksGroup tasksGroup, string description, IWorkTaskProducer workTaskProducer);
     }
}