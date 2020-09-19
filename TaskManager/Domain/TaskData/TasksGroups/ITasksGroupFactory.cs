namespace TaskData.TasksGroups
{
     public interface ITasksGroupFactory
     {
          ITasksGroup Create(string groupName);
     }
}