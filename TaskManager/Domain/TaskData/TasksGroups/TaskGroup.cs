using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.TasksGroups
{
    internal class TaskGroup : ITasksGroup
    {
        [JsonIgnore]
        private readonly ILogger<TaskGroup> mLogger;
        [JsonIgnore]
        private readonly IWorkTaskFactory mWorkTaskFactory;
        [JsonProperty]
        internal readonly Dictionary<string, IWorkTask> mTasksChildren = new Dictionary<string, IWorkTask>();

        public string ID { get; }
        public string Name { get; }

        [JsonIgnore]
        public int Size => mTasksChildren.Count;

        [JsonIgnore]
        public bool IsFinished { get => mTasksChildren.All(task => task.Value.IsFinished); }

        internal TaskGroup(string id, string groupName, IWorkTaskFactory workTaskFactory, ILogger<TaskGroup> logger)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = groupName ?? throw new ArgumentNullException(nameof(groupName));
            mWorkTaskFactory = workTaskFactory ?? throw new ArgumentNullException(nameof(workTaskFactory));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mLogger.LogDebug($"New group id {ID} created with name: {Name}");
        }

        [JsonConstructor]
        internal TaskGroup(string id, string groupName, Dictionary<string, IWorkTask> taskChildren, ILogger<TaskGroup> logger)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = groupName ?? throw new ArgumentNullException(nameof(groupName));
            mTasksChildren = taskChildren ?? throw new ArgumentNullException(nameof(taskChildren));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mLogger.LogDebug($"Task group {ID} {groupName} restored");
        }

        public IWorkTask CreateTask(string description)
        {
            IWorkTask createdTask = mWorkTaskFactory.Create(Name, description);
            AddTask(createdTask);

            return createdTask;
        }

        public IEnumerable<IWorkTask> GetAllTasks()
        {
            return mTasksChildren.Values.AsEnumerable();
        }

        public IWorkTask GetTask(string id)
        {
            if (!mTasksChildren.TryGetValue(id, out IWorkTask task))
                mLogger.LogWarning($"Task id {id} was not found in group {Name}");

            return task;
        }

        public void AddTask(IWorkTask task)
        {
            if (mTasksChildren.ContainsKey(task.ID))
            {
                mLogger.LogWarning($"Task {task.ID}, '{task.Description}' is already found in group {Name}");
                return;
            }

            task.GroupName = Name;
            mTasksChildren.Add(task.ID, task);
            mLogger.LogDebug($"Task {task.ID}, '{task.Description}' added to group {Name}");
        }

        public void RemoveTask(string id)
        {
            if (!mTasksChildren.ContainsKey(id))
            {
                mLogger.LogWarning($"Task id {id} was not found in group {Name}");
                return;
            }

            mTasksChildren.Remove(id);
            mLogger.LogDebug($"Task id {id} removed from group {Name}");
        }

        public void RemoveTask(params string[] ids)
        {
            foreach (string id in ids)
            {
                RemoveTask(id);
            }
        }

        public void UpdateTask(IWorkTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!mTasksChildren.ContainsKey(task.ID))
            {
                mLogger.LogWarning($"Task id {task.ID} is not in group {Name}. Update failed");
                return;
            }

            mTasksChildren[task.ID] = task;
            mLogger.LogDebug($"Task {task.ID}, {task.Description} updated in group {Name}. Update success");
        }
    }
}