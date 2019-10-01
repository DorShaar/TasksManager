using Logger.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.Contracts;

[assembly: InternalsVisibleTo("Statistics")]
namespace TaskData
{
    public class TaskGroup : ITaskGroup
    {
        [JsonProperty]
        private readonly ILogger mLogger;
        [JsonProperty]
        internal readonly Dictionary<string, ITask> mTasksChildren = new Dictionary<string, ITask>();

        public string ID { get; }
        public string GroupName { get; set; }

        [JsonIgnore]
        public int Size => mTasksChildren.Count;

        [JsonIgnore]
        public bool IsFinished { get => mTasksChildren.Where(task => !task.Value.IsFinished).Count() > 0 ? false : true; }

        internal TaskGroup(string groupName, ILogger logger)
        {
            mLogger = logger;
            GroupName = groupName;
            ID = IDProducer.IDProducer.ProduceID();

            mLogger?.Log($"New group id {ID} created with name: {GroupName}");
        }

        [JsonConstructor]
        internal TaskGroup(ILogger logger, Dictionary<string, ITask> taskChildren, string id, string groupName)
        {
            mLogger = logger;
            mTasksChildren = taskChildren;
            ID = id;
            GroupName = groupName;

            mLogger?.Log($"Group id {ID} restored with name: {GroupName}");
        }

        public ITask CreateTask(string description)
        {
            Task createdTask = new Task(GroupName, description, mLogger);
            AddTask(createdTask);
            return createdTask;
        }

        public IEnumerable<ITask> GetAllTasks()
        {
            return mTasksChildren.Values.AsEnumerable();
        }

        public ITask GetTask(string id)
        {
            if (!mTasksChildren.TryGetValue(id, out ITask task))
                mLogger?.LogInformation($"Task id {id} was not found in group {GroupName}");

            return task;
        }

        public void AddTask(ITask task)
        {
            if (mTasksChildren.ContainsKey(task.ID))
            {
                mLogger?.Log($"Task {task.ID}, {task.Description} is already found in group {GroupName}");
                return;
            }

            task.Group = GroupName;
            mTasksChildren.Add(task.ID, task);
            mLogger?.Log($"Task {task.ID}, {task.Description} added to group {GroupName}");
        }

        public void RemoveTask(string id)
        {
            if (!mTasksChildren.ContainsKey(id))
            {
                mLogger?.Log($"Task id {id} was not found in group {GroupName}");
                return;
            }

            mTasksChildren.Remove(id);
            mLogger?.Log($"Task id {id} removed from group {GroupName}");
        }

        public void RemoveTask(params string[] ids)
        {
            foreach (string id in ids)
            {
                RemoveTask(id);
            }
        }

        public void UpdateTask(ITask task)
        {
            if (!mTasksChildren.ContainsKey(task.ID))
            {
                mLogger?.LogError($"Task id {task.ID} is not in group {GroupName}. Update failed");
                return;
            }

            mTasksChildren[task.ID] = task;
            mLogger?.LogError($"Task {task.ID}, {task.Description} updated in group {GroupName}. Update success");
        }
    }
}