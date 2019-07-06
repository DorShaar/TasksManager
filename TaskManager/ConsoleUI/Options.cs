using CommandLine;

namespace ConsoleUI
{
     internal class Options
     {
          // Tasks Group.
          [Verb("create-group", HelpText = "Create tasks group")]
          public class CreateNewTaskGroupOptions
          {
               [Value(0)]
               public string TaskGroupName{ get; set; }
          }

          [Verb("Remove-group", HelpText = "Removes tasks group")]
          public class RemoveTaskGroupOptions
          {
               [Option('n', "name", HelpText = "Remove task group by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Remove task group by id")]
               public string TaskGroupId { get; set; }
          }

          [Verb("groups", HelpText = "Removes tasks group")]
          public class GatAllTaskGroupOptions
          {

          }

          // Tasks.

          public class CreateNewTaskOptions
          {
               [Option('n', "name", HelpText = "Remove task by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Remove task by id")]
               public string TaskGroupId { get; set; }
          }

          public class GetAllTasksOptions
          {
               [Option('n', "name", HelpText = "Get all tasks by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Get all tasks by id")]
               public string TaskGroupId { get; set; }
          }
     }
}