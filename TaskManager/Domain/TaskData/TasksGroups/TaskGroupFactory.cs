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
        private readonly ILoggerFactory mLoggerFactory;

        public TaskGroupFactory(IIDProducer idProducer, IWorkTaskFactory workTaskFactory, ILoggerFactory loggerFactory)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mWorkTaskFactory = workTaskFactory ?? throw new ArgumentNullException(nameof(workTaskFactory));
            mLoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ITasksGroup Create(string groupName)
        {
            return new TaskGroup(mIDProducer.ProduceID(), groupName, mWorkTaskFactory, mLoggerFactory.CreateLogger<TaskGroup>());
        }
    }
}