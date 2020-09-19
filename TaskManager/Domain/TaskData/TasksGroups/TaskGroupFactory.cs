using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using TaskData.IDsProducer;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("TaskData.Tests")]
[assembly: InternalsVisibleTo("TaskManager.Integration.Tests")]
namespace TaskData.TasksGroups
{
    internal class TaskGroupFactory : ITasksGroupFactory
    {
        private readonly IIDProducer mIDProducer;
        private readonly IWorkTaskFactory mWorkTaskFactory;
        private readonly ILogger<TaskGroupFactory> mLogger;

        public TaskGroupFactory(IIDProducer idProducer, IWorkTaskFactory workTaskFactory, ILogger<TaskGroupFactory> logger)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mWorkTaskFactory = workTaskFactory ?? throw new ArgumentNullException(nameof(workTaskFactory));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITasksGroup Create(string groupName)
        {
            TaskGroup taskGroup = new TaskGroup(mIDProducer.ProduceID(), groupName, mWorkTaskFactory);
            mLogger.LogDebug($"New group id {taskGroup.ID} created with name: {taskGroup.Name}");

            return taskGroup;
        }
    }
}