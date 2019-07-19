using System.Collections.Generic;

namespace TaskData.Contracts
{
    public interface ITaskGroup
    {
        string ID { get; }
        string GroupName { get; set; }
        int Size { get; }
        bool IsFinished { get; }

        IEnumerable<ITask> GetAllTasks();
        ITask GetTask(string id);
        ITask CreateTask(string description);
        void AddTask(ITask task);
        void RemoveTask(string id);
        void RemoveTask(params string[] ids);
        void UpdateTask(ITask task);
    }
}