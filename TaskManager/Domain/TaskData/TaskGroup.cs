using Logger.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskData.Contracts;

[assembly: InternalsVisibleTo("Statistics")]
namespace TaskData
{
    public class TaskGroup : ITasksGroup
    {
        [JsonProperty]
        private readonly ILogger mLogger;
        [JsonProperty]
        internal readonly Dictionary<string, IWorkTask> mTasksChildren = new Dictionary<string, IWorkTask>();

        public string ID { get; }
        public string Name { get; set; }

        [JsonIgnore]
        public int Size => mTasksChildren.Count;

        [JsonIgnore]
        public bool IsFinished { get => mTasksChildren.Where(task => !task.Value.IsFinished).Count() > 0 ? false : true; }

        internal TaskGroup(string groupName, ILogger logger)
        {
            mLogger = logger;
            Name = groupName;
            ID = IDProducer.IDProducer.ProduceID();

            mLogger?.Log($"New group id {ID} created with name: {Name}");
        }

        [JsonConstructor]
        internal TaskGroup(ILogger logger, Dictionary<string, IWorkTask> taskChildren, string id, string groupName)
        {
            mLogger = logger;
            mTasksChildren = taskChildren;
            ID = id;
            Name = groupName;

            mLogger?.Log($"Group id {ID} restored with name: {Name}");
        }

        public IWorkTask CreateTask(string description)
        {
            WorkTask createdTask = new WorkTask(Name, description, mLogger);
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
                mLogger?.LogInformation($"Task id {id} was not found in group {Name}");

            return task;
        }

        public void AddTask(IWorkTask task)
        {
            if (mTasksChildren.ContainsKey(task.ID))
            {
                mLogger?.Log($"Task {task.ID}, '{task.Description}' is already found in group {Name}");
                return;
            }

            task.GroupName = Name;
            mTasksChildren.Add(task.ID, task);
            mLogger?.Log($"Task {task.ID}, '{task.Description}' added to group {Name}");
        }

        public void RemoveTask(string id)
        {
            if (!mTasksChildren.ContainsKey(id))
            {
                mLogger?.Log($"Task id {id} was not found in group {Name}");
                return;
            }

            mTasksChildren.Remove(id);
            mLogger?.Log($"Task id {id} removed from group {Name}");
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
            if (!mTasksChildren.ContainsKey(task.ID))
            {
                mLogger?.LogError($"Task id {task.ID} is not in group {Name}. Update failed");
                return;
            }

            mTasksChildren[task.ID] = task;
            mLogger?.LogError($"Task {task.ID}, {task.Description} updated in group {Name}. Update success");
        }
    }
}