using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.OperationResults;
using TaskData.WorkTasks;
using Triangle;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.TasksGroups
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TasksGroup : ITasksGroup
    {
        [JsonProperty]
        public readonly Dictionary<string, IWorkTask> TasksChildren = new Dictionary<string, IWorkTask>();

        [JsonProperty]
        public string ID { get; }

        [JsonProperty]
        public string Name { get; private set; }

        public int Size => TasksChildren.Count;

        public bool IsFinished { get => TasksChildren.All(task => task.Value.IsFinished); }

        /// <summary>
        /// Use the IWorkTaskProducer instead.
        /// </summary>
        public TasksGroup(string id, string groupName)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = groupName ?? throw new ArgumentNullException(nameof(groupName));
        }

        /// <summary>
        /// Use the IWorkTaskProducer instead. For Json construction only.
        /// </summary>
        [JsonConstructor]
        public TasksGroup(string id, string name, Dictionary<string, IWorkTask> tasksChildren)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TasksChildren = tasksChildren ?? throw new ArgumentNullException(nameof(tasksChildren));
        }

        public IEnumerable<IWorkTask> GetAllTasks()
        {
            return TasksChildren.Values.AsEnumerable();
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
            {
                return new OperationResult(false, $"Task {task.ID}, " +
                    $"'{task.Description}' is already found in group {Name}");
            }

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

        public OperationResult SetGroupName(string newGroupName)
        {
            if (string.IsNullOrEmpty(newGroupName))
                return new OperationResult(false, $"{nameof(newGroupName)} is null or empty. Set name not performed");

            string oldName = new string(Name);
            Name = newGroupName;
            return new OperationResult(true, $"Name changed from {oldName} to {Name}");
        }

        public OperationResult SetMeasurement(string taskId, TaskTriangle taskTriangle)
        {
            OperationResult<IWorkTask> getResult = GetTask(taskId);

            if (!getResult.Success)
                return new OperationResult(false, $"{getResult.Reason}. Measurement not added");

            IWorkTask workTask = getResult.Value;

            workTask.SetMeasurement(taskTriangle);
            return new OperationResult(true, "Set new measurement");
        }
    }
}