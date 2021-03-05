using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using TaskData.IDsProducer;
using TaskData.OperationResults;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("TaskData.Tests")]
[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
[assembly: InternalsVisibleTo("ObjectSerializer.JsonService.Tests")]
namespace TaskData.TasksGroups
{
    internal class TaskGroupFactory : ITasksGroupFactory
    {
        private readonly IIDProducer mIDProducer;
        private readonly IWorkTaskProducer mWorkTaskProducer;
        private readonly ILogger<TaskGroupFactory> mLogger;

        public TaskGroupFactory(IIDProducer idProducer, IWorkTaskProducer workTaskProducer, ILogger<TaskGroupFactory> logger)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mWorkTaskProducer = workTaskProducer ?? throw new ArgumentNullException(nameof(workTaskProducer));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public OperationResult<ITasksGroup> CreateGroup(string groupName)
        {
            TaskGroup taskGroup = new TaskGroup(mIDProducer.ProduceID(), groupName);
            mLogger.LogInformation($"New group id {taskGroup.ID} created with name: {taskGroup.Name}");

            return new OperationResult<ITasksGroup>(true, taskGroup);
        }

        public OperationResult<IWorkTask> CreateTask(ITasksGroup tasksGroup, string description)
        {
            IWorkTask createdTask = mWorkTaskProducer.ProduceTask(mIDProducer.ProduceID(), description);

            OperationResult addTaskResult = tasksGroup.AddTask(createdTask);
            addTaskResult.Log(mLogger);

            if (!addTaskResult.Success)
                return new OperationResult<IWorkTask>(false, addTaskResult.Reason, createdTask);

            return new OperationResult<IWorkTask>(true, addTaskResult.Reason, createdTask);
        }
    }
}