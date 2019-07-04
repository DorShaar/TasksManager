using TaskData;

namespace Statistics
{
     public class Statistics
     {
          public int GetNumberOfTasks(TaskGroup taskGroup)
          {
               return taskGroup.mTasksChildren.Count;
          }
     }
}
