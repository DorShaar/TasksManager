using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using TaskData.IDsProducer;
using TaskData.OperationResults;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("TaskData.Tests")]
[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
namespace TaskData.TasksGroups
{
    internal class TaskGroupFactory : ITasksGroupFactory
    {
        private readonly IIDProducer mIDProducer;
        private readonly ILogger<TaskGroupFactory> mLogger;

        public TaskGroupFactory(IIDProducer idProducer, ILogger<TaskGroupFactory> logger)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITasksGroup CreateGroup(string groupName)
        {
            TaskGroup taskGroup = new TaskGroup(mIDProducer.ProduceID(), groupName);
            mLogger.LogDebug($"New group id {taskGroup.ID} created with name: {taskGroup.Name}");

            return taskGroup;
        }

        public IWorkTask CreateTask(ITasksGroup tasksGroup, string description)
        {
            OperationResult<IWorkTask> workTaskResult = tasksGroup.CreateTask(mIDProducer.ProduceID(), description);
            workTaskResult.Log(mLogger);

            return workTaskResult.Value;
        }
    }
}