using CommandLine;

namespace ConsoleUI
{
    internal class TaskOptions
    {
        // Tasks Group.
        [Verb("create-group", HelpText = "Create tasks group")]
        public class CreateNewTaskGroupOptions
        {
            [Value(0, HelpText = "Name of new task group")]
            public string TaskGroupName { get; set; }
        }

        [Verb("remove-group", HelpText = "Removes tasks group")]
        public class RemoveTaskGroupOptions
        {
            [Value(0, HelpText = "Remove task group")]
            public string TaskGroup { get; set; }

            [Option('h', "hard", HelpText = "Remove task group with all inner tasks")]
            public bool ShouldHardDelete { get; set; }
        }

        [Verb("get-groups", HelpText = "Get all groups")]
        public class GatAllTaskGroupOptions
        {
            [Option('a', "all", HelpText = "Print all groups, even groups which all their tasks are closed")]
            public bool ShouldPrintAll { get; set; }

            [Option('d', "detail", HelpText = "Print more information about each group")]
            public bool IsDetailed { get; set; }
        }

        // Tasks.

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

        // Notes.

        [Verb("create-note", HelpText = "Create note to a given task or tasks group")]
        public class CreateNoteOptions
        {
            [Value(0, HelpText = "Task or group id to create the note")]
            public string TaskId { get; set; }

            [Value(1, HelpText = "Text to write in the note")]
            public string Text { get; set; }
        }

        [Verb("open-note", HelpText = "Open note with the default text editor")]
        public class OpenNoteOptions
        {
            [Value(0, HelpText = "Task or group id to open the note")]
            public string TaskId { get; set; }
        }

        [Verb("get-note", HelpText = "Get note string text")]
        public class GetNoteOptions
        {
            [Value(0, HelpText = "Task or group id to get the note text")]
            public string TaskId { get; set; }
        }
    }
}