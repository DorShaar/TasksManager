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

          [Verb("remove-group", HelpText = "Removes tasks group")]
          public class RemoveTaskGroupOptions
          {
               [Option('n', "name", HelpText = "Remove task group by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Remove task group by id")]
               public string TaskGroupId { get; set; }
          }

          [Verb("get-groups", HelpText = "Get all groups")]
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

               [Option('n', "name", HelpText = "Add created task to tasks group by name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Add created task to tasks group by id")]
               public string TaskGroupId { get; set; }
          }

          [Verb("get-tasks", HelpText = "Get all task")]
          public class GetAllTasksOptions
          {
               [Option('n', "name", HelpText = "Get all tasks by group name")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Get all tasks by group id")]
               public string TaskGroupId { get; set; }

               [Option('d', "detail", HelpText = "Print more information about each task")]
               public bool IsDetailed { get; set; }
          }

          [Verb("close-task", HelpText = "Marks task status as closed")]
          public class CloseTasksOptions
          {
               [Value(0)]
               public string TaskId { get; set; }
          }

          [Verb("remove-task", HelpText = "Removes task")]
          public class RemoveTaskOptions
          {
               [Value(0, HelpText = "Task id to remove")]
               public string TaskId { get; set; }
          }

          [Verb("move-task", HelpText = "Moves a task to a given group")]
          public class MoveTaskOptions
          { 
               [Value(0, HelpText = "Task id to move")]
               public string TaskId { get; set; }

               [Option('n', "name", HelpText = "Task group name to move the task to")]
               public string TaskGroupName { get; set; }

               [Option('i', "id", HelpText = "Task group id to move the task to")]
               public string TaskGroupId { get; set; }
          }
     }
}