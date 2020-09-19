using TaskData.WorkTasks;

namespace TaskData.TasksGroups
{
     public interface ITasksGroupFactory
     {
          ITasksGroup CreateGroup(string groupName);
          IWorkTask CreateTask(ITasksGroup tasksGroup, string description);
     }
}