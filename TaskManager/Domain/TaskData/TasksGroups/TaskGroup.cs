using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.OperationResults;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.TasksGroups
{
    internal class TaskGroup : ITasksGroup
    {
        [JsonIgnore]
        private readonly IWorkTaskFactory mWorkTaskFactory;
        
        public readonly Dictionary<string, IWorkTask> TasksChildren = new Dictionary<string, IWorkTask>();
        public string ID { get; }
        public string Name { get; }

        [JsonIgnore]
        public int Size => TasksChildren.Count;

        [JsonIgnore]
        public bool IsFinished { get => TasksChildren.All(task => task.Value.IsFinished); }

        internal TaskGroup(string id, string groupName, IWorkTaskFactory workTaskFactory)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = groupName ?? throw new ArgumentNullException(nameof(groupName));
            mWorkTaskFactory = workTaskFactory ?? throw new ArgumentNullException(nameof(workTaskFactory));
        }

        [JsonConstructor]
        internal TaskGroup(string id, string groupName, Dictionary<string, IWorkTask> tasksChildren)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = groupName ?? throw new ArgumentNullException(nameof(groupName));
            TasksChildren = tasksChildren ?? throw new ArgumentNullException(nameof(tasksChildren));
        }

        public IEnumerable<IWorkTask> GetAllTasks()
        {
            return TasksChildren.Values.AsEnumerable();
        }

        public OperationResult<IWorkTask> CreateTask(string description)
        {
            IWorkTask createdTask = mWorkTaskFactory.Create(Name, description);

            OperationResult addTaskResult = AddTask(createdTask);
            if (!addTaskResult.Success)
                return new OperationResult<IWorkTask>(false, addTaskResult.Reason, createdTask);

            return new OperationResult<IWorkTask>(true, addTaskResult.Reason, createdTask);
        }

        public OperationResult<IWorkTask> GetTask(string id)
        {
            if (!TasksChildren.TryGetValue(id, out IWorkTask task))
                return new OperationResult<IWorkTask>(false, $"Task id {id} was not found in group {Name}", task);

            return new OperationResult<IWorkTask>(true, task);
        }

        public OperationResult AddTask(IWorkTask task)
        {
            if (TasksChildren.ContainsKey(task.ID))
                return new OperationResult(false, $"Task {task.ID}, '{task.Description}' is already found in group {Name}");

            task.GroupName = Name;
            TasksChildren.Add(task.ID, task);
            return new OperationResult(true, $"Task {task.ID}, '{task.Description}' added to group {Name}");
        }

        public OperationResult RemoveTask(string id)
        {
            if (!TasksChildren.ContainsKey(id))
                return new OperationResult(false, $"Task id {id} was not found in group {Name}");

            TasksChildren.Remove(id);
            return new OperationResult(true, $"Task id {id} removed from group {Name}");
        }

        public OperationResult UpdateTask(IWorkTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!TasksChildren.ContainsKey(task.ID))
                return new OperationResult(false, $"Task id {task.ID} is not in group {Name}. Update failed");

            TasksChildren[task.ID] = task;
            return new OperationResult(true, $"Task {task.ID}, {task.Description} updated in group {Name}. Update success");
        }
    }
}