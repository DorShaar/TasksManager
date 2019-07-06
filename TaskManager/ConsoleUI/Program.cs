using Composition;
using TaskManager.Contracts;

namespace ConsoleUI
{
     public class Program
     {
          public static void Main()
          {
               TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
               ITaskManager taskManager = serviceProvider.GetService(typeof(ITaskManager)) as ITaskManager;
          }
     }
}