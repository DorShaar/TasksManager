using CommandLine;

namespace ConsoleUI.Options
{
    internal class TaskGroupOptions
    {
        [Verb("create group", HelpText = "Create tasks group")]
        public class CreateNewTaskGroupOptions
        {
            [Value(0, HelpText = "Name of new task group")]
            public string TaskGroupName { get; set; }
        }

        [Verb("remove group", HelpText = "Removes tasks group")]
        public class RemoveTaskGroupOptions
        {
            [Value(0, HelpText = "Remove task group")]
            public string TaskGroup { get; set; }

            [Option('h', "hard", HelpText = "Remove task group with all inner tasks")]
            public bool ShouldHardDelete { get; set; }
        }

        [Verb("get groups", HelpText = "Get all groups")]
        public class GatAllTaskGroupOptions
        {
            [Option('a', "all", HelpText = "Print all groups, even groups which all their tasks are closed")]
            public bool ShouldPrintAll { get; set; }

            [Option('d', "detail", HelpText = "Print more information about each group")]
            public bool IsDetailed { get; set; }
        }
    }
}