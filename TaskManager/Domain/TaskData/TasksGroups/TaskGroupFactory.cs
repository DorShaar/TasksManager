using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using TaskData.IDsProducer;
using TaskData.OperationResults;
using TaskData.TasksGroups.Producers;
using TaskData.WorkTasks;
using TaskData.WorkTasks.Producers;

[assembly: InternalsVisibleTo("TaskData.Tests")]
[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
[assembly: InternalsVisibleTo("ObjectSerializer.JsonService.Tests")]
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

        public OperationResult<ITasksGroup> CreateGroup(string groupName, ITasksGroupProducer tasksGroupProducer)
        {
            ITasksGroup taskGroup = tasksGroupProducer.CreateGroup(mIDProducer.ProduceID(), groupName);
            mLogger.LogInformation($"New group id {taskGroup.ID} created with name: {taskGroup.Name}");

            return new OperationResult<ITasksGroup>(true, taskGroup);
        }

        public OperationResult<IWorkTask> CreateTask(ITasksGroup tasksGroup, string description, IWorkTaskProducer workTaskProducer)
        {
            if (workTaskProducer == null)
                throw new ArgumentNullException(nameof(workTaskProducer));

            IWorkTask createdTask = workTaskProducer.ProduceTask(mIDProducer.ProduceID(), description);

            OperationResult addTaskResult = tasksGroup.AddTask(createdTask);
            addTaskResult.Log(mLogger);

            if (!addTaskResult.Success)
                return new OperationResult<IWorkTask>(false, addTaskResult.Reason, createdTask);

            return new OperationResult<IWorkTask>(true, addTaskResult.Reason, createdTask);
        }
    }
}