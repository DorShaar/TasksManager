using CommandLine;

namespace ConsoleUI.Options
{
    internal class TaskOptions
    {       
        [Verb("create-task", HelpText = "Create task")]
        public class CreateNewTaskOptions
        {
            [Value(0, HelpText = "Name of new task")]
            public string Description { get; set; }

            [Value(1, HelpText = "Add created task to given tasks group")]
            public string TaskGroup { get; set; }
        }

        [Verb("get-tasks", HelpText = "Get all task")]
        public class GetAllTasksOptions
        {
            [Value(0, HelpText = "Task group name or id")]
            public string TaskGroup { get; set; }

            [Option('a', "all", HelpText = "Print all tasks, even the closed ones")]
            public bool ShouldPrintAll { get; set; }

            [Option('s', "days", HelpText = "Print all tasks which by given status")]
            public string Status { get; set; }

            [Option('h', "hours", HelpText = "Print all tasks from the last given hours")]
            public int Hours { get; set; }

            [Option('y', "days", HelpText = "Print all tasks from the last given days")]
            public int Days { get; set; }

            [Option('d', "detail", HelpText = "Print more information about each task")]
            public bool IsDetailed { get; set; }
        }

        [Verb("close-task", HelpText = "Marks task status as closed")]
        public class CloseTasksOptions
        {
            [Value(0, HelpText = "Id of task to close")]
            public string TaskId { get; set; }
        }

        [Verb("remove-task", HelpText = "Removes task")]
        public class RemoveTaskOptions
        {
            [Value(0, HelpText = "Task id to remove")]
            public string TaskId { get; set; }
        }

        [Verb("reopen-task", HelpText = "Reopen a closed task")]
        public class ReOpenTaskOptions
        {
            [Value(0, HelpText = "Task id to open again")]
            public string TaskId { get; set; }
        }

        [Verb("work-task", HelpText = "Mark task as on work")]
        public class OnWorkTaskOptions
        {
            [Value(0, HelpText = "Task id to mark as on work")]
            public string TaskId { get; set; }
        }

        [Verb("move-task", HelpText = "Moves a task to a given group")]
        public class MoveTaskOptions
        {
            [Value(0, HelpText = "Task id to move")]
            public string TaskId { get; set; }

            [Value(0, HelpText = "Task group to move the task to")]
            public string TaskGroup { get; set; }
        }

        [Verb("info-task", HelpText = "Get task information")]
        public class GetInformationTaskOptions
        {
            [Value(0, HelpText = "Task id to get information")]
            public string TaskId { get; set; }
        }
    }
}