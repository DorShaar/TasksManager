using Microsoft.Extensions.Logging;
using System;
using TaskData.IDsProducer;

namespace TaskData.WorkTasks
{
    internal class WorkTaskFactory : IWorkTaskFactory
    {
        private readonly IIDProducer mIDProducer;
        private readonly ILoggerFactory mLoggerFactory;

        public WorkTaskFactory(IIDProducer idProducer, ILoggerFactory loggerFactory)
        {
            mIDProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mLoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IWorkTask Create(string groupName, string description)
        {
            return new WorkTask(mIDProducer.ProduceID(), groupName, description, mLoggerFactory.CreateLogger<WorkTask>());
        }
    }
}