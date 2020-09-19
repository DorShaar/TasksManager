using Microsoft.Extensions.Logging;
using System;
using TaskData.IDsProducer;

namespace TaskData.WorkTasks
{
    internal class WorkTaskFactory : IWorkTaskFactory
    {
        private readonly IIDProducer mIDProducer;
        private readonly ILogger<WorkTaskFactory> mLogger;

        public WorkTaskFactory(IIDProducer idProducer, ILogger<WorkTaskFactory> logger)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IWorkTask Create(string groupName, string description)
        {
            IWorkTask workTask = new WorkTask(mIDProducer.ProduceID(), groupName, description);
            mLogger.LogDebug($"New task id {workTask.ID} created with description: {workTask.Description} created" +
                $"for group {workTask.GroupName}");

            return workTask;
        }
    }
}