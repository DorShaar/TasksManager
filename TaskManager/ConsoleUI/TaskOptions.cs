using CommandLine;

namespace ConsoleUI
{
     internal class TaskOptions
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

          [Verb("groups", HelpText = "Get all groups")]
          public class GatAllTaskGroupOptions
          {
               [Option('d', "detail", HelpText = "Print more information about each group")]
               public bool IsDetailed{ get; set; }
          }

          // Tasks.

          [Verb("create-task", HelpText = "Create task")]
          public class CreateNewTaskOptions
          {
               [Value(0)]
               public string Description { get; set; }

               [Option('n', "name", HelpText = "Remove task by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Remove task by id")]
               public string TaskGroupId { get; set; }
          }

          [Verb("tasks", HelpText = "Get all task")]
          public class GetAllTasksOptions
          {
               [Option('n', "name", HelpText = "Get all tasks by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Get all tasks by id")]
               public string TaskGroupId { get; set; }

               [Option('d', "detail", HelpText = "Print more information about each task")]
               public bool IsDetailed { get; set; }
          }
     }
}