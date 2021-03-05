using System.Collections.Generic;
using TaskData.MeasurableTasks;
using TaskData.OperationResults;
using TaskData.WorkTasks;

namespace TaskData.TasksGroups
{
    public interface ITasksGroup : IMeasurementAdder
    {
        string ID { get; }
        string Name { get; }
        int Size { get; }
        bool IsFinished { get; }

        IEnumerable<IWorkTask> GetAllTasks();
        OperationResult<IWorkTask> GetTask(string id);
        OperationResult AddTask(IWorkTask task);
        OperationResult RemoveTask(string id);
        OperationResult UpdateTask(IWorkTask task);
        OperationResult SetGroupName(string newGroupName);
    }
}