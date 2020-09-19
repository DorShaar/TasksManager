using System.Collections.Generic;
using TaskData.WorkTasks;

namespace TaskData.TasksGroups
{
    public interface ITasksGroup
    {
        string ID { get; }
        string Name { get; }
        int Size { get; }
        bool IsFinished { get; }

        IEnumerable<IWorkTask> GetAllTasks();
        IWorkTask GetTask(string id);
        IWorkTask CreateTask(string description);
        void AddTask(IWorkTask task);
        void RemoveTask(string id);
        void RemoveTask(params string[] ids);
        void UpdateTask(IWorkTask task);
    }
}