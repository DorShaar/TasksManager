using TaskData.Contracts;

namespace Statistics
{
     public class Statistics
     {
          public int GetNumberOfTasks(ITaskGroup taskGroup)
          {
               return taskGroup.Size;
          }
     }
}