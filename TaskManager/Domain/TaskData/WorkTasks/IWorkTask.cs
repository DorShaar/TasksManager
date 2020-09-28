using TaskData.MeasurableTasks;
using TaskData.OperationResults;
using TaskData.TaskStatus;

namespace TaskData.WorkTasks
{
    public interface IWorkTask : IMeasurableTask
    {
        string ID { get; }
        string GroupName { get; set; }
        string Description { get; set; }
        bool IsFinished { get; }
        Status Status { get; }

        ITaskStatusHistory TaskStatusHistory { get; }

        OperationResult CloseTask(string reason);
        OperationResult ReOpenTask(string reason);
        OperationResult MarkTaskOnWork(string reason);

        // Private Notes.
        OperationResult CreateNote(string noteDirectoryPath, string content);
        OperationResult OpenNote();
        OperationResult<string> GetNote();
    }
}