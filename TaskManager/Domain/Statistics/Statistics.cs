using TaskData.Contracts;

namespace Statistics
{
     public class Statistics
     {
          public int GetNumberOfTasks(ITasksGroup taskGroup)
          {
               return taskGroup.Size;
          }
     }
}