using System;

namespace TaskData.TaskStatus
{
    public interface ITaskStatusHistory
    {
        DateTime TimeCreated { get; }
        DateTime TimeLastOpened { get; }
        DateTime TimeLastOnWork { get; }
        DateTime TimeClosed { get; }
        Status CurrentStatus { get; }
        void AddHistory(DateTime dateTime, Status status, string reason);
    }
}